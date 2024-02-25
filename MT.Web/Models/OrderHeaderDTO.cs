namespace MT.Web.Models;

public class OrderHeaderDTO
{
    public int OrderHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double OrderTotal { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }
    public string? UserPhone { get; set; }
    public DateTime OrderTime { get; set; }
    public string? Status { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripSessionId { get; set; }
    public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
}