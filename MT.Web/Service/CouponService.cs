﻿using MT.Web.Models;
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
    public async Task<ResponseDto?> CreateCouponAsync(CouponDTO coupon)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.CouponAPIBase}/api/coupon/create",
            Data = coupon
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{SD.CouponAPIBase}/api/coupon/{id}"
        });
    }

    public async Task<ResponseDto?> GetAllCouponAsync()
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/coupon"
        });
    }

    public async Task<ResponseDto?> GetCouponAsync(string couponCode)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/coupon/getbycode/{couponCode}"
        });
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.CouponAPIBase}/api/coupon/{id}"
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDTO coupon)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.PUT,
            Url = $"{SD.CouponAPIBase}/api/coupon/update",
            Data = coupon
        });
    }
}