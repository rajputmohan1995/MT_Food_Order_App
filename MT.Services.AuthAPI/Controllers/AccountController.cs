using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MT.Services.AuthAPI.DBContext;
using MT.Services.AuthAPI.Models.DTO;

namespace MT.Services.AuthAPI.Controllers;

[Authorize]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly AuthDbContext _authDbContext;
    ResponseDto _responseDto;

    public AccountController(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
        _responseDto = new ResponseDto();
    }

    [HttpGet]
    [Route("get-user")]
    public async Task<ResponseDto> GetUserDetails(string userId)
    {
        try
        {
            var userDetails = await _authDbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (userDetails == null)
                _responseDto.Message = "User not found!";
            else
            {
                _responseDto.IsSuccess = true;
                var userDTO = new UserDTO()
                {
                    ID = userDetails.Id,
                    Name = userDetails.Name,
                    Email = userDetails.Email,
                    PhoneNumber = userDetails.PhoneNumber,
                    BillingAddress = userDetails.BillingAddress,
                    BillingCity = userDetails.BillingCity,
                    BillingCountry = userDetails.BillingCountry,
                    BillingState = userDetails.BillingState,
                    BillingZipCode = userDetails.BillingZipCode,
                    ShippingAddress = userDetails.ShippingAddress,
                    ShippingCity = userDetails.ShippingCity,
                    ShippingCountry = userDetails.ShippingCountry,
                    ShippingState = userDetails.ShippingState,
                    ShippingZipCode = userDetails.ShippingZipCode,
                };

                _responseDto.Result = userDTO;
            }
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }

    [HttpPost]
    [Route("save-user")]
    public async Task<ResponseDto> SaveUserDetails([FromBody] UserDTO user)
    {
        try
        {
            var userDetails = await _authDbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == user.ID);
            if (userDetails == null)
                _responseDto.Message = "User not found!";
            else
            {
                userDetails.Name = user.Name;
                userDetails.PhoneNumber = user.PhoneNumber;
                userDetails.BillingAddress = user.BillingAddress;
                userDetails.BillingCity = user.BillingCity;
                userDetails.BillingState = user.BillingState;
                userDetails.BillingCountry = user.BillingCountry;
                userDetails.BillingZipCode = user.BillingZipCode;
                userDetails.ShippingAddress = user.ShippingAddress;
                userDetails.ShippingCity = user.ShippingCity;
                userDetails.ShippingState = user.ShippingState;
                userDetails.ShippingCountry = user.ShippingCountry;
                userDetails.ShippingZipCode = user.ShippingZipCode;

                await _authDbContext.SaveChangesAsync();
                _responseDto.IsSuccess = true;
                _responseDto.Result = user;
            }
        }
        catch (Exception ex)
        {
            _responseDto.Message = ex.Message;
            _responseDto.IsSuccess = false;
        }
        return _responseDto;
    }
}