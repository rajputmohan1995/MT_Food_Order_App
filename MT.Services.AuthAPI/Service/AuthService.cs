using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MT.Services.AuthAPI.DBContext;
using MT.Services.AuthAPI.Models;
using MT.Services.AuthAPI.Models.DTO;
using MT.Services.AuthAPI.Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MT.Services.AuthAPI.Service;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AuthDbContext context, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager, IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> Register(RegistrationDTO registration)
    {
        AppUser newUser = new AppUser()
        {
            UserName = registration.Email,
            Email = registration.Email,
            PhoneNumber = registration.PhoneNumber,
            Name = registration.Name,
            NormalizedEmail = registration.Email.ToUpper()
        };

        try
        {
            var result = await _userManager.CreateAsync(newUser, registration.Password);
            if (result.Succeeded)
                return "";
            else return result.Errors.FirstOrDefault()?.Description ?? "Error Encountered";
        }
        catch (Exception) { }

        return "Error Encountered";
    }

    public async Task<UserDTO> Login(LoginDTO login)
    {
        var appUser = _context.AppUsers.FirstOrDefault(x => x.UserName == login.Username);
        bool isValid = await _userManager.CheckPasswordAsync(appUser, login.Password);

        if (appUser == null || isValid == false)
            return new UserDTO() { Token = "" };

        // generate JWT token
        UserDTO userDTO = new UserDTO()
        {
            Email = appUser.Email,
            ID = appUser.Id,
            Name = appUser.Name,
            PhoneNumber = appUser.PhoneNumber,
        };
        userDTO.Token = GenerateJWT(appUser);
        return userDTO;
    }

    [NonAction]
    private string GenerateJWT(AppUser appUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
            new Claim(JwtRegisteredClaimNames.Name, appUser.Name)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        var appUserData = _context.AppUsers.FirstOrDefault(x => x.Email.ToLower().Trim() == email.ToLower().Trim());

        if(appUserData != null)
        {
            var isRoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!isRoleExists)
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(appUserData, roleName);
            return true;
        }
        return false;
    }
}