using Microsoft.EntityFrameworkCore;
using MT.Services.EmailAPI.DBContext;
using MT.Services.EmailAPI.Models;
using MT.Services.EmailAPI.Service.Interfaces;
using MT.Services.EmailAPI.Services.Interfaces;
using MT.Services.EmailAPI.Utility;

namespace MT.Services.EmailAPI.Services;

public class EmailService : IEmailService
{
    private DbContextOptions<EmailDbContext> _dbOptions;
    private IConfiguration _configuration;
    private IWebHostEnvironment _hostEnvironment;
    private IProductService _productService;
    private string _invoiceEmailTemplatePath;

    public EmailService(DbContextOptions<EmailDbContext> dbOptions, IProductService productService,
        IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        _dbOptions = dbOptions;
        _productService = productService;
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;

        if (_configuration != null)
        {
            var invoiceEmailPath = _configuration.GetValue<string>("EmailTemplatePath:Invoice") ?? "";
            _invoiceEmailTemplatePath = _hostEnvironment.WebRootPath + invoiceEmailPath;
        }
    }

    public async Task EmailCartAndLogAsync(ShoppingCartDTO shoppingCartDTO)
    {
        string invoiceBillingDetails = SD.InvoiceBillingDetails.Replace(SD.InvoiceFullNameReplace, shoppingCartDTO.CartHeader.UserEmail);
        invoiceBillingDetails = invoiceBillingDetails.Replace(SD.InvoiceAddressLine1Replace, "Address Line 1");
        invoiceBillingDetails = invoiceBillingDetails.Replace(SD.InvoiceAddressLine2Replace, "Address Line 2");
        invoiceBillingDetails = invoiceBillingDetails.Replace(SD.InvoiceAddressCityStateCountryZipCodeReplace, "City_State_ZipCode");
        invoiceBillingDetails = invoiceBillingDetails.Replace(SD.InvoiceEmailReplace, shoppingCartDTO.CartHeader.UserEmail);
        invoiceBillingDetails = invoiceBillingDetails.Replace(SD.InvoicePhoneReplace, "1234567890");

        string shippingBillingDetails = SD.InvoiceShippingDetails.Replace(SD.InvoiceFullNameReplace, shoppingCartDTO.CartHeader.UserEmail);
        shippingBillingDetails = shippingBillingDetails.Replace(SD.InvoiceAddressLine1Replace, "Address Line 1");
        shippingBillingDetails = shippingBillingDetails.Replace(SD.InvoiceAddressLine2Replace, "Address Line 2");
        shippingBillingDetails = shippingBillingDetails.Replace(SD.InvoiceAddressCityStateCountryZipCodeReplace, "City_State_ZipCode");
        shippingBillingDetails = shippingBillingDetails.Replace(SD.InvoiceEmailReplace, shoppingCartDTO.CartHeader.UserEmail);
        shippingBillingDetails = shippingBillingDetails.Replace(SD.InvoicePhoneReplace, "1234567890");

        string invoicePaymentInformation = SD.InvoicePaymentInformation.Replace(SD.InvoicePaymentInformationCardNameReplace, "Visa");
        invoicePaymentInformation = invoicePaymentInformation.Replace(SD.InvoicePaymentInformationCardNumberReplace, "***** 332");
        invoicePaymentInformation = invoicePaymentInformation.Replace(SD.InvoicePaymentInformationExpDateReplace, "10/" + DateTime.UtcNow.AddYears(5).Year);

        string orderPreferences = SD.InvoiceOrderPreferences.Replace(SD.InvoiceOrderPreferenceIsGiftReplace, "No");
        orderPreferences = orderPreferences.Replace(SD.InvoiceOrderPreferenceIsExpressDeliveryReplace, "Yes");
        orderPreferences = orderPreferences.Replace(SD.InvoiceOrderPreferenceHasInsuranceReplace, "No");
        orderPreferences = orderPreferences.Replace(SD.InvoiceOrderPreferenceHasCouponReplace, "No");

        string invoiceEmailContent = File.ReadAllText(_invoiceEmailTemplatePath);

        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceBillingDetailsReplace, invoiceBillingDetails);
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceShippingDetailsReplace, shippingBillingDetails);
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoicePaymentInformationReplace, invoicePaymentInformation);
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceOrderPreferencesReplace, orderPreferences);

        var allProductList = await _productService.GetProductsAsync();
        string cartDetailList = "";
        var subTotal = 0d;
        foreach (var cartDetail in shoppingCartDTO.CartDetails)
        {
            var productDetail = allProductList.FirstOrDefault(x => x.ProductId == cartDetail.ProductId);
            if (productDetail != null)
            {
                var cartItemTotal = (cartDetail.Quantity * productDetail.Price);
                cartDetailList += $"<tr><td>{productDetail.Name}</td><td class=\"text-center\">{productDetail.Price.ToString("c")}</td><td class=\"text-center\">{cartDetail.Quantity}</td><td class=\"text-right\">{cartItemTotal.ToString("c")}</td></tr>";
                subTotal += cartItemTotal;
            }
        }

        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceCartDetailsReplace, cartDetailList);

        var shippingAmount = 20;
        var discountAmount = 0;
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceSubTotal, subTotal.ToString("c"));
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceDiscountTotal, discountAmount.ToString("c"));
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceShippingTotal, shippingAmount.ToString("c"));
        invoiceEmailContent = invoiceEmailContent.Replace(SD.InvoiceFinalTotal, (subTotal + discountAmount + shippingAmount).ToString("c"));

        await LogAndEmail(invoiceEmailContent, shoppingCartDTO.CartHeader.UserEmail);
    }

    public async Task RegisterUserAndLogAsync(UserDTO userDTO)
    {
        var registerUserEmailContent = $"User Registration Successful. <br />Email: {userDTO.Email}<br />Name: {userDTO.Name}" ;
        await LogAndEmail(registerUserEmailContent, _configuration.GetValue<string>("AdminDetails:Email"));
    }

    public async Task<bool> LogAndEmail(string invoiceEmailContent, string emailAddress)
    {
        try
        {
            EmailLogger emailLogger = new EmailLogger()
            {
                Email = emailAddress,
                EmailSent = DateTime.UtcNow,
                Message = invoiceEmailContent,
            };

            await using var _db = new EmailDbContext(_dbOptions);
            await _db.EmailLoggers.AddAsync(emailLogger);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

}