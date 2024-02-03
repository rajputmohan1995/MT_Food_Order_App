using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IAuthService
{
    Task<ResponseDto?> LoginAsync(LoginDTO login);
    Task<ResponseDto?> RegisterAsync(RegistrationDTO registration);
    Task<ResponseDto?> AssignRoleAsync(RegistrationDTO assignRole);
    Task<ResponseDto?> GetUserDetailsAsync(string userId);
    Task<ResponseDto?> SaveUserDetailsAsync(UserDTO user);
    Task<ResponseDto?> ChangePasswordAsync(ChangePasswordDTO changePassword);
}