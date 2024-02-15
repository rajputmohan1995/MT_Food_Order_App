using MT.Services.OrderAPI.Models.DTO;

namespace MT.Web.Models;

public class StripeRequestDTO
{
    public string StripSessionUrl { get; set; } = string.Empty;
    public string StripSessionId { get; set; } = string.Empty;
    public string ApprovedUrl { get; set; }
    public string CancelUrl { get; set; }
    public OrderHeaderDTO OrderHeader { get; set; }
}