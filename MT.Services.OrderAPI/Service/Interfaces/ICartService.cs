namespace MT.Services.OrderAPI.Service.Interfaces;

public interface ICartService
{
    Task<bool> RemoveAllAsync(string userId);
}