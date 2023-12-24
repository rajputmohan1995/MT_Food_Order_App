using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MT.Services.CouponAPI.DBContext;
using MT.Services.CouponAPI.Models;
using MT.Services.CouponAPI.Models.DTO;

namespace MT.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
public class CouponController : ControllerBase
{
    public readonly CouponDbContext _couponDbContext;
    ResponseDto _responseDto;
    IMapper _mapper;
    public CouponController(CouponDbContext couponDbContext, IMapper mapper)
    {
        _couponDbContext = couponDbContext;
        _responseDto = new ResponseDto();
        _mapper = mapper;   
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            var objList = _couponDbContext.Coupons.ToList();
            _responseDto.Result = _mapper.Map<IEnumerable<CouponDTO>>(objList);
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
            var objData = _couponDbContext.Coupons.First(c => c.CouponId == id);
            _responseDto.Result = _mapper.Map<CouponDTO>(objData);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpGet]
    [Route("GetByCode/{code}")]
    public ResponseDto GetByCode(string code)
    {
        try
        {
            var objData = _couponDbContext.Coupons.First(c => c.CouponCode.Trim().ToLower() == code.Trim().ToLower());
            _responseDto.Result = _mapper.Map<CouponDTO>(objData);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpPost]
    [Route("Create")]
    public ResponseDto Create([FromBody] CouponDTO coupon)
    {
        try
        {
            var obj = _mapper.Map<Coupon>(coupon);
            _couponDbContext.Coupons.Add(obj);
            _couponDbContext.SaveChanges();

            _responseDto.Result = _mapper.Map<CouponDTO>(obj);
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }

    [HttpPut]
    [Route("Update")]
    public ResponseDto Update([FromBody] CouponDTO coupon)
    {
        try
        {
            var obj = _mapper.Map<Coupon>(coupon);
            _couponDbContext.Coupons.Update(obj);
            _couponDbContext.SaveChanges();

            _responseDto.Result = _mapper.Map<CouponDTO>(obj);
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
    public ResponseDto Delete(int id)
    {
        try
        {
            var coupon = _couponDbContext.Coupons.First(c => c.CouponId == id);
            _couponDbContext.Remove(coupon);
            _couponDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = ex.Message;
        }
        return _responseDto;
    }
}
