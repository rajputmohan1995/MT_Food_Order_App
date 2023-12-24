using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class CouponService : ICouponService
{
    private readonly IBaseService _baseService;
    public CouponService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public Task<ResponseDto?> CreateCouponAsync(CouponDTO coupon)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.CouponAPIBase}/api/coupon/create",
            Data = coupon
        });
    }

    public Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{SD.CouponAPIBase}/api/coupon/{id}"
        });
    }

    public Task<ResponseDto?> GetAllCouponAsync()
    {
        return _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/coupon"
        });
    }

    public Task<ResponseDto?> GetCouponAsync(string couponCode)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/getbycode/{couponCode}"
        });
    }

    public Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/coupon/{id}"
        });
    }

    public Task<ResponseDto?> UpdateCouponAsync(CouponDTO coupon)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.PUT,
            Url = $"{SD.CouponAPIBase}/api/coupon/update",
            Data = coupon
        });
    }
}