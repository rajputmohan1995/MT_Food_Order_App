using Microsoft.EntityFrameworkCore;
using MT.Services.OrderAPI.Models;

namespace MT.Services.OrderAPI.DBContext;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> context) : base(context)
    { }

    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
}