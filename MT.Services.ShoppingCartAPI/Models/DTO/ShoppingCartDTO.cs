using MT.Services.EmailAPI.Models;

namespace MT.Services.ShoppingCartAPI.Models.DTO;

public class ShoppingCartDTO
{
    public bool DirectUpdate { get; set; }
    public CartHeaderDTO CartHeader { get; set; }
    public IEnumerable<CartDetailDTO> CartDetails { get; set; }
    public UserDTO? User { get; set; }
}