using Microsoft.EntityFrameworkCore;
using MT.Services.EmailAPI.Models;

namespace MT.Services.EmailAPI.DBContext;

public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> context) : base(context)
    { }

    public DbSet<EmailLogger> EmailLoggers { get; set; }
}