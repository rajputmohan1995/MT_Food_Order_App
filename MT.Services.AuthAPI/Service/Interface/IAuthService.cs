using MT.Services.AuthAPI.Models;
using MT.Services.AuthAPI.Models.DTO;

namespace MT.Services.AuthAPI.Service.Interface;

public interface IAuthService
{
    Task<string> Register(RegistrationDTO registration);
    Task<UserDTO> Login(LoginDTO login);
    Task<string> ChangePassword(ChangePasswordDTO changePasswordDTO);
    Task<bool> AssignRole(string email, string roleName);
}