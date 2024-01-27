namespace MT.Services.EmailAPI.Utility;

public class SD
{
    public const string InvoiceBillingDetailsReplace = "[{InvoiceBillingDetails}]";
    public const string InvoiceShippingDetailsReplace = "[{InvoiceShippingDetails}]";
    public const string InvoicePaymentInformationReplace = "[{InvoicePaymentInformation}]";
    public const string InvoiceOrderPreferencesReplace = "[{InvoiceOrderPreferences}]";
    public const string InvoiceCartDetailsReplace = "[{InvoiceCartDetails}]";

    public const string InvoiceBillingDetails = $"<strong>{InvoiceFullNameReplace}:</strong><br>{InvoiceAddressLine1Replace}<br>{InvoiceAddressLine2Replace}<br>{InvoiceAddressCityStateCountryZipCodeReplace}<br><strong>{InvoicePhoneReplace}</strong><br><strong>{InvoiceEmailReplace}</strong>";
    public const string InvoiceFullNameReplace = "[{FullName}]";
    public const string InvoiceAddressLine1Replace = "[{AddressLine1}]";
    public const string InvoiceAddressLine2Replace = "[{AddressLine2}]";
    public const string InvoiceAddressCityStateCountryZipCodeReplace = "[{City_State_Country_ZipCode}]";
    public const string InvoicePhoneReplace = "[{Phone}]";
    public const string InvoiceEmailReplace = "[{Email}]";

    public const string InvoiceShippingDetails = InvoiceBillingDetails;

    public const string InvoiceOrderPreferences = $"<strong>Gift: </strong>{InvoiceOrderPreferenceIsGiftReplace}<br><strong>Express Delivery:</strong> {InvoiceOrderPreferenceIsExpressDeliveryReplace}<br><strong>Insurance:</strong> {InvoiceOrderPreferenceHasCouponReplace}<br><strong>Coupon:</strong> {InvoiceOrderPreferenceHasCouponReplace}<br>";
    public const string InvoiceOrderPreferenceIsGiftReplace = "[{IsGift}]";
    public const string InvoiceOrderPreferenceIsExpressDeliveryReplace = "[{IsExpressDelivery}]";
    public const string InvoiceOrderPreferenceHasInsuranceReplace = "[{HasInsurance}]";
    public const string InvoiceOrderPreferenceHasCouponReplace = "[{HasCoupon}]";

    public const string InvoicePaymentInformation = $"<strong>Card Name:</strong> {InvoicePaymentInformationCardNameReplace}<br><strong>Card Number:</strong> {InvoicePaymentInformationCardNumberReplace}<br><strong>Exp Date:</strong> {InvoicePaymentInformationExpDateReplace}<br>";
    public const string InvoicePaymentInformationCardNameReplace = "[{CardName}]";
    public const string InvoicePaymentInformationCardNumberReplace = "[{CardNumber}]";
    public const string InvoicePaymentInformationExpDateReplace = "[{ExpDate}]";

    public const string InvoiceSubTotal = "[{SubTotal}]";
    public const string InvoiceDiscountTotal = "[{DiscountTotal}]";
    public const string InvoiceShippingTotal = "[{ShippingTotal}]";
    public const string InvoiceFinalTotal = "[{FinalTotal}]";
}