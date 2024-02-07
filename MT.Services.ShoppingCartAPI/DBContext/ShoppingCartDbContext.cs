using Microsoft.EntityFrameworkCore;
using MT.Services.ShoppingCartAPI.Models;

namespace MT.Services.CouponAPI.DBContext;

public class ShoppingCartDbContext : DbContext
{
    public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> context) : base(context)
    { }

    public DbSet<CartHeader> CartHeaders { get; set; }
    public DbSet<CartDetail> CartDetails { get; set; }
}