using Microsoft.AspNetCore.Identity;

namespace MT.Services.AuthAPI.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
    public string BillingAddress { get; set; }
    public string BillingCity { get; set; }
    public string BillingState { get; set; }
    public string BillingCountry { get; set; }
    public string BillingZipCode { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
    public string ShippingCountry { get; set; }
    public string ShippingZipCode { get; set; }
}