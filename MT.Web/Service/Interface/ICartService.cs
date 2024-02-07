using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface ICartService
{
    Task<ResponseDto?> UpsertCartAsync(ShoppingCartDTO shoppingCartDTO);
    Task<ResponseDto?> RemoveItemAsync(int cartHeaderId, int cartDetailId);
    Task<ResponseDto?> GetCartByUserIdAsync(string userId, bool loadUser = false);
    Task<ResponseDto?> ApplyCouponAsync(ShoppingCartDTO shoppingCartDTO);
    Task<ResponseDto?> EmailCartAsync(ShoppingCartDTO shoppingCartDTO);
}