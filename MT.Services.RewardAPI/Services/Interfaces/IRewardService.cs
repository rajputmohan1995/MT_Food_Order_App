using MT.Services.RewardAPI.Message;

namespace MT.Services.RewardAPI.Services.Interfaces;

public interface IRewardService
{
    Task UpdateRewards(RewardMessage rewardMessage);
}