using System.ComponentModel.DataAnnotations;

namespace MT.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; set; }
    [Required]
    public string? CouponCode { get; set; }
    [Required]
    public double DiscountAmount { get; set; }
    public int MinimumAmount { get; set; }
}