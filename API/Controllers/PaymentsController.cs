using API.Extensions;
using API.SignalR;
using Core;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.interfaces;
using Core.spec;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;


public class PaymentsController(IPaymentService paymentService,
    IUnitOfWork unit , ILogger<PaymentsController> logger
     , IConfiguration config,
     IHubContext<NotificationHub> hubContext
     ) : BaseApiController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymenyIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymenyIntent(cartId);
        if(cart == null) return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unit.Repo<DeliveryMethod>().ListAllAsync());
    }
    [HttpPost("webhook")]
public async Task<IActionResult> StripeWebhook()
{
    var json = await new StreamReader(Request.Body).ReadToEndAsync();
    
    logger.LogInformation("Webhook received. Body length: {Length}", json.Length);
    logger.LogInformation("Stripe-Signature header: {Header}", 
        Request.Headers["Stripe-Signature"].ToString());

    try
    {
        var stripeEvent = ConstructStripeEvent(json);
        logger.LogInformation("Event constructed successfully. Type: {Type}", stripeEvent.Type);

        if (stripeEvent.Data.Object is not PaymentIntent intent)
        {
            logger.LogWarning("Event data is not PaymentIntent. Actual type: {Type}", 
                stripeEvent.Data.Object?.GetType().Name);
            return BadRequest("Invalid event data");
        }

        logger.LogInformation("PaymentIntent Id: {Id}, Status: {Status}, Amount: {Amount}", 
            intent.Id, intent.Status, intent.Amount);

        await HandlePaymentIntentSucceeded(intent);

        return Ok();
    }
    catch (StripeException ex)
    {
        logger.LogError(ex, "Stripe webhook error: {Message}", ex.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
        return StatusCode(500, "Webhook processing failed");
    }
}


    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            logger.LogInformation("Looking for order with PaymentIntentId: {Id}", intent.Id);

            var spec = new OrderSpecification(intent.Id, true);
            var order = await unit.Repo<Core.Entities.OrderAggregate.Order>()
                .GetEntityWithSpec(spec);

            if (order == null)
            {
                logger.LogError("Order not found for PaymentIntentId: {Id}", intent.Id);
                throw new Exception($"Order not found for PaymentIntentId: {intent.Id}");
            }

            logger.LogInformation("Order found. Current Status: {Status}, Total: {Total}",
                order.Status, order.GetTotal());

            // ===== المقارنة الصحيحة =====
            var orderAmountInCents = (long)Math.Round(
                order.GetTotal() * 100m, 
                MidpointRounding.AwayFromZero);

            if (orderAmountInCents != intent.Amount)
            {
                logger.LogWarning(
                    "Payment mismatch. Order total (cents): {OrderTotal}, Intent amount: {IntentAmount}",
                    orderAmountInCents, intent.Amount);

                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
                logger.LogInformation("Order status updated to PaymentReceived");
            }
            // ============================

            await unit.Complete();

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId)
                    .SendAsync("OrderCompleteNotification", order.ToDto());
            }
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _whSecret,
                throwOnApiVersionMismatch: false
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to construct stripe event");
            throw new StripeException("Invalid signature");
        }
    }
}
