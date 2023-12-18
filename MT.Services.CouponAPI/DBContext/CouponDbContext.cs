using Microsoft.EntityFrameworkCore;
using MT.Services.CouponAPI.Models;

namespace MT.Services.CouponAPI.DBContext;

public class CouponDbContext : DbContext
{
    public CouponDbContext(DbContextOptions<CouponDbContext> context) : base(context)
    { }

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 1,
            CouponCode = "10OFF",
            DiscountAmount = 10,
            MinimumAmount = 30
        });

        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 2,
            CouponCode = "20OFF",
            DiscountAmount = 20,
            MinimumAmount = 50
        });
    }
}