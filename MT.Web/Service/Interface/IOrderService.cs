using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrderAsync(ShoppingCartDTO shoppingCartDTO);
    Task<ResponseDto?> CreatePaymentSessionAsync(StripeRequestDTO stripeRequestDTO);
    Task<ResponseDto?> ValidatePaymentSessionAsync(int orderHeaderId);
}