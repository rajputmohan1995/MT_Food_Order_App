using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrderAsync(ShoppingCartDTO shoppingCartDTO);
    Task<ResponseDto?> CreatePaymentSessionAsync(StripeRequestDTO stripeRequestDTO);
    Task<ResponseDto?> ValidatePaymentSessionAsync(int orderHeaderId);
    Task<ResponseDto?> GetAllOrdersAsync(string userId, string orderStatus);
    Task<ResponseDto?> GetOrderByIdAsync(int orderId, string userId);
    Task<ResponseDto?> UpdateOrderStatusAsync(int orderId, string userId, Utility.SD.OrderStatus orderStatus);

}