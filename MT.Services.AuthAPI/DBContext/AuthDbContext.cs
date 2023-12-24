using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MT.Services.AuthAPI.Models;

namespace MT.Services.AuthAPI.DBContext;

public class AuthDbContext : IdentityDbContext<AppUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> context) : base(context)
    { }

    public DbSet<AppUser> AppUsers { get; set; }
}