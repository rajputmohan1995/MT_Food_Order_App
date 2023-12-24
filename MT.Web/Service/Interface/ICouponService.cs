using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface ICouponService
{
    Task<ResponseDto?> GetCouponAsync(string couponCode);
    Task<ResponseDto?> GetCouponByIdAsync(int id);
    Task<ResponseDto?> GetAllCouponAsync();
    Task<ResponseDto?> CreateCouponAsync(CouponDTO coupon);
    Task<ResponseDto?> UpdateCouponAsync(CouponDTO coupon);
    Task<ResponseDto?> DeleteCouponAsync(int id);
}