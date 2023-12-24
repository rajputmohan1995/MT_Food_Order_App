using Microsoft.AspNetCore.Identity;

namespace MT.Services.AuthAPI.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
}