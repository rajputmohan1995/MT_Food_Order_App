using Microsoft.EntityFrameworkCore;
using MT.Services.CouponAPI.Models;

namespace MT.Services.CouponAPI.DBContext;

public class CouponDbContext : DbContext
{
    public CouponDbContext(DbContextOptions<CouponDbContext> context) : base(context)
    { }

    public DbSet<Coupon> Coupons { get; set; }
}