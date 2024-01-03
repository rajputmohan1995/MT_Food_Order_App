namespace MT.Services.ShoppingCartAPI.Models.DTO;

public class ShoppingCartDTO
{
    public CartHeaderDTO CartHeader { get; set; }
    public IEnumerable<CartDetailDTO> CartDetails { get; set; }
}