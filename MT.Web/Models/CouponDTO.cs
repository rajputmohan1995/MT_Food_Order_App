using System.ComponentModel.DataAnnotations;

namespace MT.Web.Models;

public class CouponDTO
{
    public int CouponId { get; set; }
    [Required(ErrorMessage = "Coupon code is required")]
    public string? CouponCode { get; set; }
    [Required(ErrorMessage = "Discount amount is required")]
    public double DiscountAmount { get; set; }
    [Required(ErrorMessage = "Minimum amount is required")]
    public int MinimumAmount { get; set; }
}