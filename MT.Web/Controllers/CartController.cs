using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

[Authorize]
public class CartController : BaseController
{
    readonly ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        ShoppingCartDTO model = new ShoppingCartDTO();
        try
        {
            var userId = GetLoggedInUserId();
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


    [HttpPost]
    public async Task<IActionResult> Upsert(ProductDetailDTO productDetailDTO)
    {
        try
        {
            var shoppingCartObj = new ShoppingCartDTO()
            {
                CartHeader = new() { UserId = GetLoggedInUserId(), },
                CartDetails = new List<CartDetailDTO>
                {
                    new() { ProductId = productDetailDTO.ProductId, Quantity = productDetailDTO.Quantity }
                }
            };

            var saveCartResponse = await _cartService.UpsertCartAsync(shoppingCartObj);
            if (saveCartResponse != null)
            {
                if (saveCartResponse.IsSuccess)
                {
                    TempData["success"] = "Item added in successfully";
                    return RedirectToAction(nameof(Index), "Cart");
                }
                else
                    TempData["error"] = saveCartResponse.Message;
            }
            else TempData["error"] = "Internal error occured while adding item in the cart, please try again.";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("Detail", "Product", new { id = productDetailDTO.ProductId });
    }

    [Route("UpdateItemQty/{cartDetailId:int}/{isIncrement:bool?}/{directUpdate:bool?}")]
    public async Task<IActionResult> UpdateItemQty(int cartDetailId, bool? isIncrement, bool? directUpdate, ShoppingCartDTO shoppingCartDTO)
    {
        try
        {
            var shoppingCartObj = new ShoppingCartDTO()
            {
                DirectUpdate = directUpdate == true,
                CartHeader = shoppingCartDTO.CartHeader,
                CartDetails = shoppingCartDTO.CartDetails.Where(x => x.CartDetailId == cartDetailId).ToList()
            };

            if (directUpdate == null || directUpdate == false)
                shoppingCartObj.CartDetails.First().Quantity = isIncrement == true ? 1 : -1;

            var cartItemQtyUpdateResult = await _cartService.UpsertCartAsync(shoppingCartObj);
            if (cartItemQtyUpdateResult != null)
            {
                if (cartItemQtyUpdateResult.IsSuccess)
                {
                    TempData["success"] = "Item quantity updated successfully";
                    return RedirectToAction(nameof(Index), "Cart");
                }
                else
                    TempData["error"] = cartItemQtyUpdateResult.Message;
            }
            else TempData["error"] = "Internal error occured while updating item quantity, please try again.";

        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [Route("remove/{cartHeaderId:int}/{cartDetailId:int}")]
    public async Task<IActionResult> Remove(int cartHeaderId, int cartDetailId)
    {
        try
        {
            var removeItemResult = await _cartService.RemoveItemAsync(cartHeaderId, cartDetailId);
            if (removeItemResult != null)
            {
                if (removeItemResult.IsSuccess)
                    TempData["success"] = "Item removed successfully";
                else TempData["error"] = removeItemResult.Message;
            }
            else TempData["error"] = "Internal error occured while removing item from the cart";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(ShoppingCartDTO shoppingCartDTO)
    {
        try
        {
            shoppingCartDTO.CartDetails = new List<CartDetailDTO>();
            var removeItemResult = await _cartService.ApplyCouponAsync(shoppingCartDTO);
            if (removeItemResult != null)
            {
                if (removeItemResult.IsSuccess)
                    TempData["success"] = "Coupon applied successfully";
                else TempData["error"] = removeItemResult.Message;
            }
            else TempData["error"] = "Internal error occured while applying the coupon";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EmailCart(ShoppingCartDTO shoppingCartDTO)
    {
        try
        {
            shoppingCartDTO.CartHeader.UserId = GetLoggedInUserId();
            shoppingCartDTO.CartHeader.UserEmail = GetLoggedInEmailId();

            var removeItemResult = await _cartService.EmailCartAsync(shoppingCartDTO);
            if (removeItemResult != null)
            {
                if (removeItemResult.IsSuccess)
                    TempData["success"] = "Cart details send successfully to registered email. You'll shortly receive an email.";
                else TempData["error"] = removeItemResult.Message;
            }
            else TempData["error"] = "Internal error occured while sending an email for cart details";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("Index");
    }
}