using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions;


public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            BuyerEmail = order.BuyerEmail,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            PaymentSummary = order.PaymentSummary,
            DeliveryMethod = order.DeliveryMethod,
            ShippingPrice = order.DeliveryMethod.Price,
            Status = order.Status,
            OrderItems = order.OrderItems
                .Select(x => x.ToDto())
                .ToList(),
            Subtotal = order.Subtotal,
            PaymentIntentId = order.PaymentIntentId,
            Total = order.GetTotal()
        };
    }

    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            ProductId = orderItem.ItemOrdered.ProductId,
            ProductName = orderItem.ItemOrdered.ProductName,
            PictureUrl = orderItem.ItemOrdered.PictureUrl,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        };
    }
}