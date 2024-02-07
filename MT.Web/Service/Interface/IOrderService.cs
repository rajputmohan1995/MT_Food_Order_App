using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IOrderService
{
    Task<ResponseDto?> CreateOrderAsync(ShoppingCartDTO shoppingCartDTO);
}