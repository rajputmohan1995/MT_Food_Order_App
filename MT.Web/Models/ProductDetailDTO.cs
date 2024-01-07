namespace MT.Web.Models;

public class ProductDetailDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public ProductDTO ProductDetail { get; set; }
    public List<ProductDTO> RelatedProducts { get; set;}
}