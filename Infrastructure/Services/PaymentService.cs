using Core;
using Core.Entities;
using Core.interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(
    IConfiguration config,
    ICartService cartService,
    IUnitOfWork unit
) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymenyIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        var cart = await cartService.GetCartAsync(cartId);
        if (cart == null) return null;

        decimal shippingPrice = 0m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repo<DeliveryMethod>()
                .GetByIdAsync((int)cart.DeliveryMethodId);

            if (deliveryMethod == null) return null;

            shippingPrice = deliveryMethod.Price;
        }

        // دائمًا نأخذ السعر الحالي من قاعدة البيانات
        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repo<Core.Entities.Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return null;

            item.Price = productItem.Price;
        }

        // ===== الحساب الصحيح =====
        var subtotal = cart.Items.Sum(item => item.Quantity * item.Price);
        var total = subtotal + shippingPrice;

        // نحول المبلغ النهائي إلى سنتات بطريقة صحيحة
        var amountInCents = (long)Math.Round(total * 100m, MidpointRounding.AwayFromZero);
        // =========================

        var service = new PaymentIntentService();
        PaymentIntent intent;

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                AllowedPaymentMethodTypes = ["card"]
            };

            intent = await service.CreateAsync(options);

            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            // نجيب الـ intent الحالي أولاً
            intent = await service.GetAsync(cart.PaymentIntentId);

            if (intent.Status == "succeeded")
            {
                // لا يجوز إعادة استخدام PaymentIntent ناجح
                throw new InvalidOperationException(
                    "Payment has already succeeded. Create or recover the order before starting another payment.");
            }

            if (intent.Status == "canceled")
            {
                // ننشئ intent جديد
                intent = await service.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    AllowedPaymentMethodTypes = ["card"]
                });

                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                // نحدّث المبلغ فقط
                intent = await service.UpdateAsync(
                    cart.PaymentIntentId,
                    new PaymentIntentUpdateOptions { Amount = amountInCents });

                cart.ClientSecret = intent.ClientSecret;
            }
        }

        await cartService.SetCartAsync(cart);
        return cart;
    }
}