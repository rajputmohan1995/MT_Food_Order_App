namespace MT.Web.Models;

public class ProductDetailDTO
{
    public ProductDTO ProductDetail { get; set; }
    public List<ProductDTO> RelatedProducts { get; set;}
}