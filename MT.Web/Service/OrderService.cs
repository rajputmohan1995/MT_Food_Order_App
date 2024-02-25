using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;
    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> CreateOrderAsync(ShoppingCartDTO shoppingCartDTO)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/create-order",
            ApiType = SD.ApiType.POST,
            Data = shoppingCartDTO
        });
    }

    public async Task<ResponseDto?> CreatePaymentSessionAsync(StripeRequestDTO stripeRequestDTO)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/create-payment-session",
            ApiType = SD.ApiType.POST,
            Data = stripeRequestDTO
        });
    }

    public async Task<ResponseDto?> ValidatePaymentSessionAsync(int orderHeaderId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/validate-payment-session?orderHeaderId={orderHeaderId}",
            ApiType = SD.ApiType.POST,
        });
    }

    public async Task<ResponseDto?> GetAllOrdersAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/get-orders/{userId}",
            ApiType = SD.ApiType.GET
        });
    }

    public async Task<ResponseDto?> GetOrderByIdAsync(int orderId, string userId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/get-order/{orderId}/{userId}",
            ApiType = SD.ApiType.GET
        });
    }

    public async Task<ResponseDto?> UpdateOrderStatusAsync(int orderId, string userId, SD.OrderStatus orderStatus)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.OrderAPIBase}/api/order/update-order-status/{orderId}/{userId}",
            ApiType = SD.ApiType.POST,
            Data = orderStatus.ToString()
        });
    }
}