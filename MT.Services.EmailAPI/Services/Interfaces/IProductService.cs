using MT.Services.EmailAPI.Models;

namespace MT.Services.EmailAPI.Service.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
}