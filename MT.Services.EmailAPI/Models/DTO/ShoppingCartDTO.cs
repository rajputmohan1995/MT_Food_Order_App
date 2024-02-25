namespace MT.Services.EmailAPI.Models;

public class ShoppingCartDTO
{
    public bool DirectUpdate { get; set; }
    public CartHeaderDTO CartHeader { get; set; }
    public List<CartDetailDTO> CartDetails { get; set; }
    public UserDTO? User { get; set; }
}

public class CartHeaderDTO
{
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double CartTotal { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }
}

public class CartDetailDTO
{
    public int CartDetailId { get; set; }
    public int CartHeaderId { get; set; }
    public CartHeaderDTO? CartHeader { get; set; }
    public int ProductId { get; set; }
    public ProductDTO? Product { get; set; }
    public int Quantity { get; set; }
}