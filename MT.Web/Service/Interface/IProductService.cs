using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IProductService
{
    Task<ResponseDto?> GetProductByIdAsync(int id);
    Task<ResponseDto?> GetAllProductAsync();
    Task<ResponseDto?> CreateProductAsync(ProductDTO product);
    Task<ResponseDto?> UpdateProductAsync(ProductDTO product);
    Task<ResponseDto?> DeleteProductAsync(int id);
}