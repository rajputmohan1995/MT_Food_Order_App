﻿namespace MT.Services.ShoppingCartAPI.Models.DTO;

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