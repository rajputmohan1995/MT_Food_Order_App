using MT.Services.EmailAPI.Message;
using MT.Services.EmailAPI.Models;

namespace MT.Services.EmailAPI.Services.Interfaces;

public interface IEmailService
{
    Task EmailCartAndLogAsync(ShoppingCartDTO shoppingCartDTO);
    Task RegisterUserAndLogAsync(UserDTO userDTO);
    Task NewOrderEmailAsync(RewardMessage rewardMessage);
}