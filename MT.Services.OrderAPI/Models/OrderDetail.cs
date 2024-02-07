using MT.Services.OrderAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Services.OrderAPI.Models;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }
    public int OrderHeaderId { get; set; }
    [ForeignKey("OrderHeaderId")]
    public OrderHeader? OrderHeader { get; set; }
    public int ProductId { get; set; }
    [NotMapped]
    public ProductDTO? Product { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public double ProductPrice { get; set; }
}
