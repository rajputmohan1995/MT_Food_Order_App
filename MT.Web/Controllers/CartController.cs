using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MT.Web.Controllers;

public class CartController : Controller
{
    readonly ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        ShoppingCartDTO model = new ShoppingCartDTO();
        try
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var userCartDetails = await _cartService.GetCartByUserIdAsync(userId);
            if (userCartDetails != null && userCartDetails.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ShoppingCartDTO>(Convert.ToString(userCartDetails.Result));
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return View(model);
    }
}