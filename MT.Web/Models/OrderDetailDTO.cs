﻿namespace MT.Web.Models.DTO;

public class OrderDetailDTO
{
    public int OrderDetailId { get; set; }
    public int OrderHeaderId { get; set; }
    public int ProductId { get; set; }
    public ProductDTO? Product { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public double ProductPrice { get; set; }
}