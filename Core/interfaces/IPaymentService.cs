using Core.Entities;

namespace Core;

public interface IPaymentService

{
    Task<ShoppingCart?> CreateOrUpdatePaymenyIntent(string cartId);
}