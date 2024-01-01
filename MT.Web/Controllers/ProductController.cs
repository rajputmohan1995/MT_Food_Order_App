using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDTO>? productList = null;

        var responseProducts = await _productService.GetAllProductAsync();
        if (responseProducts != null && responseProducts.IsSuccess)
            productList = JsonConvert.DeserializeObject<List<ProductDTO>>(responseProducts.Result?.ToString() ?? "");
        else TempData["error"] = responseProducts?.Message ?? SD.InternalErrorOccured;

        return View(productList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductDTO product)
    {
        if (ModelState.IsValid)
        {
            var responseProduct = await _productService.CreateProductAsync(product);
            if (responseProduct != null)
            {
                if (responseProduct.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else TempData["error"] = responseProduct.Message;
            }
            else TempData["error"] = "Internal error occured while adding new product";
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var productResponse = await _productService.GetProductByIdAsync(id);
            if (productResponse != null)
            {
                if (productResponse.IsSuccess)
                {
                    var productDetail = JsonConvert.DeserializeObject<ProductDTO>(productResponse.Result.ToString() ?? "");
                    return View(productDetail);
                }
                else TempData["error"] = productResponse.Message;
            }
            else TempData["error"] = "Internal error occured while fetching product details";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return View(new ProductDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductDTO product)
    {
        if (ModelState.IsValid)
        {
            var responseProduct = await _productService.UpdateProductAsync(product);
            if (responseProduct != null)
            {
                if (responseProduct.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else TempData["error"] = responseProduct.Message;
            }
            else TempData["error"] = "Internal error occured while updating product details";
        }
        return View();
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id > 0)
        {
            var responseProduct = await _productService.GetProductByIdAsync(id);
            if (responseProduct != null && responseProduct.IsSuccess)
            {
                var productData = JsonConvert.DeserializeObject<ProductDTO>(responseProduct.Result?.ToString() ?? "");
                return View(productData);
            }
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(ProductDTO product)
    {
        if (product?.ProductId > 0)
        {
            var responseProduct = await _productService.DeleteProductAsync(product.ProductId);
            if (responseProduct != null)
            {
                if (responseProduct.IsSuccess)
                {
                    TempData["success"] = "Product deleted successfully";
                    return RedirectToAction(nameof(Index));
                }
                else TempData["error"] = responseProduct.Message;
            }
            else TempData["error"] = "Internal error occured while deleting the product";
        }
        return View(product);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Detail(int id)
    {
        var productDetail = new ProductDetailDTO();
        try
        {
            var productResponse = await _productService.GetProductByIdAsync(id);
            if (productResponse != null)
            {
                if (productResponse.IsSuccess)
                {
                    productDetail.ProductDetail = JsonConvert.DeserializeObject<ProductDTO>(productResponse.Result.ToString() ?? "");

                    var relatedProducts = await _productService.GetAllProductAsync();
                    productDetail.RelatedProducts = JsonConvert.DeserializeObject<List<ProductDTO>>(relatedProducts.Result.ToString() ?? "");
                    return View(productDetail);
                }
                else TempData["error"] = productResponse.Message;
            }
            else TempData["error"] = "Internal error occured while fetching product details";
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return View(productDetail);
    }
}
