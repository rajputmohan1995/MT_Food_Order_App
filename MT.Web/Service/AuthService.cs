using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;
    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public Task<ResponseDto?> AssignRoleAsync(RegistrationDTO assignRole)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.AuthAPIBase}/api/auth/assign-role",
            Data = assignRole
        }, withBearer: false);
    }

    public Task<ResponseDto?> LoginAsync(LoginDTO login)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.AuthAPIBase}/api/auth/login",
            Data = login
        }, withBearer: false);
    }

    public Task<ResponseDto?> RegisterAsync(RegistrationDTO registration)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.AuthAPIBase}/api/auth/register",
            Data = registration
        }, withBearer: false);
    }

    public Task<ResponseDto?> GetUserDetailsAsync(string userId)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{SD.AuthAPIBase}/api/account/get-user/{userId}"
        });
    }

    public Task<ResponseDto?> SaveUserDetailsAsync(UserDTO user)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.AuthAPIBase}/api/account/save-user",
            Data = user
        });
    }

    public Task<ResponseDto?> ChangePasswordAsync(ChangePasswordDTO changePassword)
    {
        return _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Url = $"{SD.AuthAPIBase}/api/auth/change-password",
            Data = changePassword
        });
    }
}