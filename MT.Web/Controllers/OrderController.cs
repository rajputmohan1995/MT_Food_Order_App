using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service;
using MT.Web.Service.Interface;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

[Authorize]
public class OrderController : BaseController
{
    readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var allOrdersResult = await _orderService.GetAllOrdersAsync(GetLoggedInUserId());
        if(allOrdersResult == null && allOrdersResult?.IsSuccess == false)
            return View(new List<OrderHeaderDTO>());

        return View(JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(allOrdersResult.Result?.ToString()));
    }
}