using MT.Services.ShoppingCartAPI.Models.DTO;
using MT.Services.ShoppingCartAPI.Service.Interfaces;
using Newtonsoft.Json;

namespace MT.Services.ShoppingCartAPI.Service;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<CouponDTO> GetCouponByCode(string couponCode)
    {
        var client = _httpClientFactory.CreateClient("Coupon");

        var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (responseObj?.IsSuccess == true)
        {
            return JsonConvert.DeserializeObject<CouponDTO>(responseObj.Result.ToString());
        }
        return new CouponDTO();
    }
}