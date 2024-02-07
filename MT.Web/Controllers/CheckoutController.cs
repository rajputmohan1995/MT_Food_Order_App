using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;

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
                    TempData["success"] = "Order placed successfully";
                    //return RedirectToAction("New", "Order");
                }
                else
                    TempData["error"] = orderSaveResponse?.Message ?? "Internal error occured while placing your order";
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Internal error occured: " + ex.Message;
        }
        return RedirectToAction("Index");
    }
}