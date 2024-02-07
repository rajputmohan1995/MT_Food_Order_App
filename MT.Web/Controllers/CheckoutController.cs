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
    public CheckoutController(ICartService cartService)
    {
        _cartService = cartService;
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
}