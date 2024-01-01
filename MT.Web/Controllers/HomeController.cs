using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MT.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var homeDtoResponse = new HomeDTO();
        try
        {
            var allProducts = await _productService.GetAllProductAsync();
            if (allProducts?.IsSuccess == true)
                homeDtoResponse.RecommendedProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(allProducts?.Result?.ToString() ?? "") ?? new();
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return View(homeDtoResponse);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
