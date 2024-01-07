using MT.Services.ShoppingCartAPI.Models.DTO;

namespace MT.Services.ShoppingCartAPI.Service.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<ProductDTO> GetProductByIdAsync(int id);
}