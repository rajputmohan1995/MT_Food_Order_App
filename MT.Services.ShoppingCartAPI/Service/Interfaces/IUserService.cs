using MT.Services.EmailAPI.Models;

namespace MT.Services.ShoppingCartAPI.Service.Interfaces;

public interface IUserService
{
    Task<UserDTO> GetUserAsync(string userId);
}