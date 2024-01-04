namespace MT.Web.Models;

public class ShoppingCartDTO
{
    public CartHeaderDTO CartHeader { get; set; }
    public IEnumerable<CartDetailDTO> CartDetails { get; set; }
}

public class CartHeaderDTO
{
    public int CartHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double CartTotal { get; set; }
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