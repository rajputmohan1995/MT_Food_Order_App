using MT.Services.AuthAPI.Models.DTO;

namespace MT.Services.AuthAPI.Service.Interface;

public interface IAuthService
{
    Task<string> Register(RegistrationDTO registration);
    Task<UserDTO> Login(LoginDTO login);
    Task<bool> AssignRole(string email, string roleName);
}