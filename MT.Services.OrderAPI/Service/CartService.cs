using MT.Services.OrderAPI.Models.DTO;
using MT.Services.OrderAPI.Service.Interfaces;
using Newtonsoft.Json;

namespace MT.Services.OrderAPI.Service;

public class CartService : ICartService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public CartService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> RemoveAllAsync(string userId)
    {
        var client = _httpClientFactory.CreateClient("ShoppingCart");

        var response = await client.PostAsync($"/api/cart/remove-all-items?userId={userId}", null);
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (responseObj?.IsSuccess == true)
        {
            return Convert.ToBoolean(responseObj.Result);
        }
        return false;
    }
}