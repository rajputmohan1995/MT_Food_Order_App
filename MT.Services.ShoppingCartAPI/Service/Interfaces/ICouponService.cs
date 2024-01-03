using MT.Services.ShoppingCartAPI.Models.DTO;

namespace MT.Services.ShoppingCartAPI.Service.Interfaces;

public interface ICouponService
{
    Task<CouponDTO> GetCouponByCode(string couponCode);
}