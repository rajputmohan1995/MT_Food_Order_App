using Microsoft.EntityFrameworkCore;
using MT.Services.RewardAPI.DBContext;
using MT.Services.RewardAPI.Message;
using MT.Services.RewardAPI.Models;
using MT.Services.RewardAPI.Services.Interfaces;
using System.Net.Mail;

namespace MT.Services.RewardAPI.Services;

public class RewardService : IRewardService
{
    private DbContextOptions<RewardsDbContext> _dbOptions;
    private IConfiguration _configuration;
    private IWebHostEnvironment _hostEnvironment;
    private string _invoiceEmailTemplatePath;

    public RewardService(DbContextOptions<RewardsDbContext> dbOptions,
        IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        _dbOptions = dbOptions;
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;

        if (_configuration != null)
        {
            var invoiceEmailPath = _configuration.GetValue<string>("EmailTemplatePath:Invoice") ?? "";
            _invoiceEmailTemplatePath = _hostEnvironment.WebRootPath + invoiceEmailPath;
        }
    }

    public async Task UpdateRewards(RewardMessage rewardMessage)
    {
        try
        {
            Rewards newReward = new Rewards()
            {
                OrderId = rewardMessage.OrderId,
                RewardsActivity = rewardMessage.RewardsActivity,
                UserId = rewardMessage.UserId,
                RewardsDateTime = DateTime.Now
            };

            await using var _db = new RewardsDbContext(_dbOptions);
            await _db.Rewards.AddAsync(newReward);
            await _db.SaveChangesAsync();
        }
        catch
        {
        }
    }
}