using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;
    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<IActionResult> Index()
    {
        List<CouponDTO>? couponList = null;

        var responseCoupons = await _couponService.GetAllCouponAsync();
        if (responseCoupons != null && responseCoupons.IsSuccess)
            couponList = JsonConvert.DeserializeObject<List<CouponDTO>>(responseCoupons.Result?.ToString() ?? "");
        else TempData["error"] = "No record found";

        return View(couponList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CouponDTO coupon)
    {
        if (ModelState.IsValid)
        {
            var responseCoupons = await _couponService.CreateCouponAsync(coupon);
            if (responseCoupons != null && responseCoupons.IsSuccess)
            {
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction(nameof(Index));
            }
            else TempData["error"] = "Internal error occured while adding new coupon";
        }
        return View();
    }

    public async Task<IActionResult> Delete(int couponId)
    {
        if (couponId > 0)
        {
            var responseCoupon = await _couponService.GetCouponByIdAsync(couponId);
            if (responseCoupon != null && responseCoupon.IsSuccess)
            {
                var couponData = JsonConvert.DeserializeObject<CouponDTO>(responseCoupon.Result?.ToString() ?? "");
                return View(couponData);
            }
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(CouponDTO coupon)
    {
        if (coupon?.CouponId > 0)
        {
            var responseCoupon = await _couponService.DeleteCouponAsync(coupon.CouponId);
            if (responseCoupon != null && responseCoupon.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            else TempData["error"] = "Internal error occured while deleting the coupon";
        }
        return View(coupon);
    }
}