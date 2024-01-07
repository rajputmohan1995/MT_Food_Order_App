using MT.Services.ShoppingCartAPI.Models.DTO;
using MT.Services.ShoppingCartAPI.Service.Interfaces;
using Newtonsoft.Json;

namespace MT.Services.ShoppingCartAPI.Service;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        var client = _httpClientFactory.CreateClient("Product");

        var response = await client.GetAsync($"/api/product");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (responseObj?.IsSuccess == true)
        {
            return JsonConvert.DeserializeObject<List<ProductDTO>>(responseObj.Result.ToString());
        }
        return new List<ProductDTO>();
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("Product");

        var response = await client.GetAsync($"/api/product/{id}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (responseObj?.IsSuccess == true)
        {
            return JsonConvert.DeserializeObject<ProductDTO>(responseObj.Result.ToString());
        }
        return new ProductDTO();
    }
}