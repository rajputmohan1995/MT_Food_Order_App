using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace MT.Web.Controllers;

public class BaseController : Controller
{
    public string? GetLoggedInUserId()
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        return userId;
    }
}