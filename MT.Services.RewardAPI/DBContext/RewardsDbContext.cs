using Microsoft.EntityFrameworkCore;
using MT.Services.RewardAPI.Models;

namespace MT.Services.RewardAPI.DBContext;

public class RewardsDbContext : DbContext
{
    public RewardsDbContext(DbContextOptions<RewardsDbContext> context) : base(context)
    { }

    public DbSet<Rewards> Rewards { get; set; }
}