using MT.Web.Models;
using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface ICartService
{
    Task<ResponseDto?> UpsertCartAsync(ShoppingCartDTO shoppingCartDTO);
    Task<ResponseDto?> RemoveItemAsync(int cartHeaderId, int cartDetailId);
    Task<ResponseDto?> GetCartByUserIdAsync(string userId);
    Task<ResponseDto?> ApplyCouponAsync(ShoppingCartDTO shoppingCartDTO);
}