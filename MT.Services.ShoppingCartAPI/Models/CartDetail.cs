using MT.Services.ShoppingCartAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Services.ShoppingCartAPI.Models;

public class CartDetail
{
    [Key]
    public int CartDetailId { get; set; }
    public int CartHeaderId { get; set; }
    [ForeignKey("CartHeaderId")]
    public CartHeader? CartHeader { get; set; }
    public int ProductId { get; set; }
    [NotMapped]
    public ProductDTO? Product { get; set; }
    public int Quantity { get; set; }
}