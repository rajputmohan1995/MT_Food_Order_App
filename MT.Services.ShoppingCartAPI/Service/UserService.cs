using MT.Services.EmailAPI.Models;
using MT.Services.ShoppingCartAPI.Service.Interfaces;
using Newtonsoft.Json;

namespace MT.Services.ShoppingCartAPI.Service;

public class UserService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<UserDTO> GetUserAsync(string userId)
    {
        var client = _httpClientFactory.CreateClient("User");

        var response = await client.GetAsync($"/api/user/{userId}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (responseObj?.IsSuccess == true)
        {
            return JsonConvert.DeserializeObject<UserDTO>(responseObj.Result.ToString());
        }
        return new UserDTO();
    }
}