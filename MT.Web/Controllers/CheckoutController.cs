using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;
using Newtonsoft.Json;
using System.Net;

namespace MT.Web.Controllers;

[Authorize]
public class CheckoutController : BaseController
{
    readonly ICartService _cartService;
    readonly IOrderService _orderService;

    public CheckoutController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        ShoppingCartDTO model = new ShoppingCartDTO();
        try
        {
            var userId = GetLoggedInUserId();
            var userCartDetails = await _cartService.GetCartByUserIdAsync(userId, loadUser: true);
            if (userCartDetails != null && userCartDetails.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ShoppingCartDTO>(Convert.ToString(userCartDetails.Result));
            }
            else return RedirectToAction("Index", "Cart");
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ShoppingCartDTO cartDto)
    {
        try
        {
            var validationResponse = ValidateShoppingCart(cartDto);
            if (!validationResponse.Item1)
            {
                TempData["error"] = "Required details are not supplied";
                TempData["ShoppingCartValidationErrors"] = validationResponse.Item2;
                return RedirectToAction("Index");
            }

            ShoppingCartDTO cartToSend = null;
            var userCartDetails = await _cartService.GetCartByUserIdAsync(GetLoggedInUserId());
            if (userCartDetails != null && userCartDetails.IsSuccess)
            {
                cartToSend = JsonConvert.DeserializeObject<ShoppingCartDTO>(Convert.ToString(userCartDetails.Result));
            }

            if (cartToSend is null)
                TempData["error"] = "Cart details not found";
            else
            {
                cartToSend.CartHeader.UserEmail = GetLoggedInEmailId();
                cartToSend.CartHeader.UserFullName = cartDto.User.Name;
                cartToSend.CartHeader.UserPhone = cartDto.User.PhoneNumber;
                cartToSend.User = new();
                cartToSend.User.Name = cartDto.User.Name;
                cartToSend.User.PhoneNumber = cartDto.User.PhoneNumber;
                cartToSend.User.Email = cartDto.User.Email;
                cartToSend.User.BillingAddress = cartDto.User.BillingAddress;
                cartToSend.User.BillingCity = cartDto.User.BillingCity;
                cartToSend.User.BillingState = cartDto.User.BillingState;
                cartToSend.User.BillingCountry = cartDto.User.BillingCountry;
                cartToSend.User.BillingZipCode = cartDto.User.BillingZipCode;

                cartToSend.User.ShippingAddress = cartDto.User.DifferentShippingAddress ? cartDto.User.ShippingAddress : cartDto.User.BillingZipCode;
                cartToSend.User.ShippingCity = cartDto.User.DifferentShippingAddress ? cartDto.User.ShippingCity : cartDto.User.BillingCity;
                cartToSend.User.ShippingState = cartDto.User.DifferentShippingAddress ? cartDto.User.ShippingState : cartDto.User.BillingState;
                cartToSend.User.ShippingCountry = cartDto.User.DifferentShippingAddress ? cartDto.User.ShippingCountry : cartDto.User.BillingCountry;
                cartToSend.User.ShippingZipCode = cartDto.User.DifferentShippingAddress ? cartDto.User.ShippingZipCode : cartDto.User.BillingZipCode;

                var orderSaveResponse = await _orderService.CreateOrderAsync(cartToSend);
                if (orderSaveResponse != null && orderSaveResponse.IsSuccess)
                {
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(orderSaveResponse.Result.ToString());

                    var stripReqDto = new StripeRequestDTO()
                    {
                        OrderHeader = orderHeaderDto,
                        UserDetails = cartToSend.User,
                        ApprovedUrl = domain + $"checkout/confirmation?orderId={orderHeaderDto.OrderHeaderId}&orderConfirmationId={Guid.NewGuid().ToString()}",
                        CancelUrl = domain + "checkout/index"
                    };

                    var createStripeSessionResponse = await _orderService.CreatePaymentSessionAsync(stripReqDto);
                    if (createStripeSessionResponse != null && createStripeSessionResponse.IsSuccess)
                    {
                        var createSessionDto = JsonConvert.DeserializeObject<StripeRequestDTO>(createStripeSessionResponse.Result.ToString());
                        Response.Headers.Add("Location", createSessionDto.StripSessionUrl);
                        return new StatusCodeResult((int)HttpStatusCode.RedirectMethod);
                    }
                    else TempData["error"] = orderSaveResponse?.Message ?? "Internal error occured while placing your order";

                }
                else TempData["error"] = orderSaveResponse?.Message ?? "Internal error occured while placing your order";
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Internal error occured: " + ex.Message;
        }
        return RedirectToAction("Index");
    }


    [NonAction]
    private Tuple<bool, string> ValidateShoppingCart(ShoppingCartDTO cartDto)
    {
        var errorList = "<ul>";
        var isSuccess = true;
        if (cartDto == null || cartDto?.User == null)
        {
            isSuccess = false;
            errorList += "<li>Required details not supplied</li>";
        }

        if (string.IsNullOrWhiteSpace(cartDto?.User?.Name))
        {
            isSuccess = false;
            errorList += "<li>Name is required</li>";
        }

        if (string.IsNullOrWhiteSpace(cartDto?.User?.PhoneNumber))
        {
            isSuccess = false;
            errorList += "<li>Phone is required</li>";
        }

        if (string.IsNullOrWhiteSpace(cartDto?.User?.Email))
        {
            isSuccess = false;
            errorList += "<li>Email is required</li>";
        }

        if (string.IsNullOrWhiteSpace(cartDto?.User?.BillingAddress) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.BillingCity) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.BillingState) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.BillingCountry) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.BillingZipCode))
        {
            isSuccess = false;
            errorList += "<li>Complete billing address is required</li>";
        }

        if (string.IsNullOrWhiteSpace(cartDto?.User?.ShippingAddress) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.ShippingCity) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.ShippingState) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.ShippingCountry) ||
            string.IsNullOrWhiteSpace(cartDto?.User?.ShippingZipCode))
        {
            isSuccess = false;
            errorList += "<li>Complete shipping address is required</li>";
        }

        errorList += "</ul>";
        return new Tuple<bool, string>(isSuccess, errorList);
    }


    public async Task<IActionResult> Confirmation(int orderId, string orderConfirmationId)
    {
        var orderWithPaymentDetails = await _orderService.ValidatePaymentSessionAsync(orderId);

        if (orderWithPaymentDetails == null || !orderWithPaymentDetails.IsSuccess)
        {
            TempData["error"] = "Payment unsuccessful";
            return RedirectToAction("Index");
        }

        var objOrderHeaderWithPaymentDetail = JsonConvert.DeserializeObject<OrderHeaderDTO>(orderWithPaymentDetails.Result.ToString());

        if (objOrderHeaderWithPaymentDetail?.Status == SD.OrderStatus.Approved.ToString())
        {
            TempData["success"] = "Payment approved";
            return View(orderId);
        }

        TempData["error"] = "Payment failed";
        return RedirectToAction("Index");
    }
}