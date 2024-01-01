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
    public ResponseDto Create([FromBody] ProductDTO product)
    {
        try
        {
            var obj = _mapper.Map<Product>(product);
            _productDbContext.Products.Add(obj);
            _productDbContext.SaveChanges();

            _responseDto.Result = _mapper.Map<ProductDTO>(obj);
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
    public ResponseDto Update([FromBody] ProductDTO product)
    {
        try
        {
            var obj = _mapper.Map<Product>(product);
            _productDbContext.Products.Update(obj);
            _productDbContext.SaveChanges();

            _responseDto.Result = _mapper.Map<ProductDTO>(obj);
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
}
