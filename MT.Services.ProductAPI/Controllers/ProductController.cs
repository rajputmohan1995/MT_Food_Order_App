using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Services.ProductAPI.DBContext;
using MT.Services.ProductAPI.Models;
using MT.Services.ProductAPI.Models.DTO;
using MT.Services.ProductAPI.Utility;

namespace MT.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    public readonly ProductDbContext _productDbContext;
    ResponseDto _responseDto;
    IMapper _mapper;
    public ProductController(ProductDbContext productDbContext, IMapper mapper)
    {
        _productDbContext = productDbContext;
        _responseDto = new ResponseDto();
        _mapper = mapper;
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            var objList = _productDbContext.Products.ToList();
            _responseDto.Result = _mapper.Map<IEnumerable<ProductDTO>>(objList);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpGet]
    [Route("{id:int}")]
    public ResponseDto Get(int id)
    {
        try
        {
            var productData = _productDbContext.Products.First(x => x.ProductId == id);
            _responseDto.Result = _mapper.Map<ProductDTO>(productData);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles = SD.RoleAdmin)]
    public async Task<ResponseDto> Create(ProductDTO productDto)
    {
        try
        {
            var productObj = _mapper.Map<Product>(productDto);
            _productDbContext.Products.Add(productObj);
            _productDbContext.SaveChanges();

            if (productDto.Image != null)
            {
                productObj = AddProductImageInLocalPath(productObj, productDto);
            }
            else productObj.ImageUrl = "https://placehold.co/600x400";

            _productDbContext.Products.Update(productObj);
            await _productDbContext.SaveChangesAsync();

            _responseDto.Result = _mapper.Map<ProductDTO>(productObj);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpPut]
    [Route("update")]
    [Authorize(Roles = SD.RoleAdmin)]
    public async Task<ResponseDto> Update(ProductDTO productDto)
    {
        try
        {
            var productObj = _mapper.Map<Product>(productDto);
            _productDbContext.Products.Update(productObj);
            await _productDbContext.SaveChangesAsync();

            if (productDto.Image != null)
            {
                DeleteProductImage(productObj);
                productObj = AddProductImageInLocalPath(productObj, productDto);
            }

            _productDbContext.Products.Update(productObj);
            await _productDbContext.SaveChangesAsync();

            _responseDto.Result = _mapper.Map<ProductDTO>(productObj);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = SD.RoleAdmin)]
    public ResponseDto Delete(int id)
    {
        try
        {
            var removeObj = _productDbContext.Products.First(p => p.ProductId == id);

            DeleteProductImage(removeObj);
            _productDbContext.Products.Remove(removeObj);
            _productDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }


    [NonAction]
    private bool DeleteProductImage(Product removeObj)
    {
        if (removeObj is not null && !string.IsNullOrWhiteSpace(removeObj.ImageLocalPathUrl))
        {
            var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), removeObj.ImageLocalPathUrl);
            FileInfo file = new FileInfo(oldFilePathDirectory);
            if (file.Exists) file.Delete();
        }

        return true;
    }

    [NonAction]
    private Product AddProductImageInLocalPath(Product productObj, ProductDTO productDTO)
    {
        string fileName = productObj.ProductId + Path.GetExtension(productDTO?.Image?.FileName);
        string filePath = @"wwwroot\ProductImages\" + fileName;

        var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
        using var fileStream = new FileStream(filePathDirectory, FileMode.Create);
        productDTO?.Image?.CopyTo(fileStream);

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
        productObj.ImageUrl = baseUrl + "/ProductImages/" + fileName;
        productObj.ImageLocalPathUrl = filePath;

        return productObj;
    }
}
