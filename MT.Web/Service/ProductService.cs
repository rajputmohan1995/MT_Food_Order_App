using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class ProductService : IProductService
{
    private readonly IBaseService _baseService;
    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public Task<ResponseDto?> CreateProductAsync(ProductDTO product)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.ProductAPIBase}/api/product/create",
            Data = product
        });
    }

    public Task<ResponseDto?> DeleteProductAsync(int id)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{SD.ProductAPIBase}/api/product/{id}"
        });
    }

    public Task<ResponseDto?> GetAllProductAsync()
    {
        return _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ProductAPIBase}/api/product"
        });
    }

    public Task<ResponseDto?> GetProductByIdAsync(int id)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            Url = $"{SD.ProductAPIBase}/api/product/{id}"
        });
    }

    public Task<ResponseDto?> UpdateProductAsync(ProductDTO product)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.PUT,
            Url = $"{SD.ProductAPIBase}/api/product/update",
            Data = product
        });
    }
}