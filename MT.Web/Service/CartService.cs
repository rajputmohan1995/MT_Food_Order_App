using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;
    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> ApplyCouponAsync(ShoppingCartDTO shoppingCartDTO)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ShoppingCartAPIBase}/api/cart/apply-coupon",
            ApiType = SD.ApiType.POST,
            Data = shoppingCartDTO
        });
    }

    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId, bool loadUser = false)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ShoppingCartAPIBase}/api/cart/get-items/{userId}?loadUser={loadUser}",
            ApiType = SD.ApiType.GET
        });
    }

    public async Task<ResponseDto?> RemoveItemAsync(int cartHeaderId, int cartDetailId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ShoppingCartAPIBase}/api/cart/remove-item?cartHeaderId={cartHeaderId}&cartDetailId={cartDetailId}",
            ApiType = SD.ApiType.POST
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(ShoppingCartDTO shoppingCartDTO)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ShoppingCartAPIBase}/api/cart/upsert",
            ApiType = SD.ApiType.POST,
            Data = shoppingCartDTO
        });
    }

    public async Task<ResponseDto?> EmailCartAsync(ShoppingCartDTO shoppingCartDTO)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ShoppingCartAPIBase}/api/cart/email-cart-request",
            ApiType = SD.ApiType.POST,
            Data = shoppingCartDTO
        });
    }
}