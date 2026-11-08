using Core.Entities;
using Core.Entities.OrderAggregate;

namespace API.DTOs;


public class OrderDto
{
    public int Id {get;set;}
     public DateTime OrderDate{get;set;} 
    public required string BuyerEmail {get;set;}
    public required ShippingAddress ShippingAddress {get;set;}
    public required DeliveryMethod DeliveryMethod{get;set;}
    public decimal ShippingPrice {get;set;}
    public PaymentSummary PaymentSummary{get;set;} = null!;

    public required List<OrderItemDto> OrderItems {get;set;}
    public decimal Subtotal {get;set;}
    public OrderStatus Status{get;set;} = OrderStatus.Pending;

    public decimal Total {get;set;}
    public required string PaymentIntentId{get;set;}
}

