using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

[Authorize]
public class OrderController : BaseController
{
    readonly IOrderService _orderService;
    readonly ResponseDto _responseDto;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
        _responseDto = new ResponseDto();
    }

    public async Task<IActionResult> Index(string orderStatus)
    {
        TempData["FilterOrderStatus"] = orderStatus;
        var allOrdersResult = await _orderService.GetAllOrdersAsync(GetLoggedInUserId(), orderStatus);
        if (allOrdersResult == null && allOrdersResult?.IsSuccess == false)
            return View(new List<OrderHeaderDTO>());

        return View(JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(allOrdersResult.Result?.ToString()));
    }

    public async Task<IActionResult> Detail(int orderId)
    {
        var userId = GetLoggedInUserId();
        var orderHeaderDto = await _orderService.GetOrderByIdAsync(orderId, userId);

        if (orderHeaderDto == null && orderHeaderDto?.IsSuccess == false)
            return NotFound();

        return View(JsonConvert.DeserializeObject<OrderHeaderDTO>(orderHeaderDto.Result?.ToString()));
    }

    [HttpPost]
    public async Task<IActionResult> OrderReadyForPickup(int orderId)
    {
        await UpdateOrderStatus(orderId, SD.OrderStatus.ReadyForPickup);
        return RedirectToAction("Detail", new { orderId = orderId });
    }

    [HttpPost]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        await UpdateOrderStatus(orderId, SD.OrderStatus.Completed);
        return RedirectToAction("Detail", new { orderId = orderId });
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        await UpdateOrderStatus(orderId, SD.OrderStatus.Canceled);
        return RedirectToAction("Detail", new { orderId = orderId });
    }


    [NonAction]
    private async Task UpdateOrderStatus(int orderId, SD.OrderStatus orderStatus)
    {
        try
        {
            var userId = GetLoggedInUserId();
            var orderStatusResponse = await _orderService.UpdateOrderStatusAsync(orderId, userId, orderStatus);
            if (orderStatusResponse != null && orderStatusResponse.IsSuccess)
            {
                TempData["success"] = $"Order updated successfully to [{orderStatus}]";
            }
            else TempData["error"] = !string.IsNullOrWhiteSpace(orderStatusResponse.Message) ? orderStatusResponse.Message : "Something went wrong while update order status";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
    }
}