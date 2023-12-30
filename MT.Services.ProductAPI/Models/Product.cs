using System.ComponentModel.DataAnnotations;

namespace MT.Services.ProductAPI.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 2)]
    public string Name { get; set; }
    [Required]
    [Range(minimum: 1, maximum: 1000)]
    public double Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
}