using System.ComponentModel.DataAnnotations;

namespace MT.Services.OrderAPI.Models.DTO;

public class ProductDTO
{
    public int ProductId { get; set; }
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "Product name must be 2-500 characters long")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Product price is required")]
    [Range(1, 1000, ErrorMessage = "Product price must be in 1-1000 range")]
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLocalPathUrl { get; set; }
}