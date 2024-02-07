using MT.Services.OrderAPI.Models.DTO;

namespace MT.Services.OrderAPI.Service.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<ProductDTO> GetProductByIdAsync(int id);
}