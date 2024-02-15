﻿using MT.Web.Models;
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
}