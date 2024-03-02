using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MT.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;

    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var loginObj = new LoginDTO();
        //loginObj.ReturnUrl = HttpContext.Request.Query["returnUrl"].ToString();
        return View(loginObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO login)
    {
        if (ModelState.IsValid)
        {
            var loginResponse = await _authService.LoginAsync(login);
            if (loginResponse != null)
            {
                if (loginResponse.IsSuccess)
                {
                    TempData["success"] = "Login Successful";
                    var userData = JsonConvert.DeserializeObject<UserDTO>(loginResponse.Result.ToString());
                    await SignInAsync(userData);
                    _tokenProvider.SetToken(userData?.Token);

                    if (!string.IsNullOrWhiteSpace(login?.ReturnUrl) && Url.IsLocalUrl(login?.ReturnUrl))
                        return Redirect(login.ReturnUrl);
                    else return RedirectToAction("Index", "Home");
                }
                else
                {
                    loginResponse.IsSuccess = false;
                    var errorMsg = "Invalid username/password";
                    ModelState.AddModelError("CustomError", errorMsg);
                    TempData["error"] = errorMsg;
                }
            }
            else
            {
                loginResponse = new() { IsSuccess = false };
                loginResponse.IsSuccess = false;
                ModelState.AddModelError("CustomError", "Internal error occured");
                TempData["error"] = "Internal error occured";
            }
        }

        login.Password = "";
        return View(login);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInAsync(UserDTO userData)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(userData.Token);
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)?.Value ?? userData.Email));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value ?? userData.ID));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name)?.Value ?? userData.Name));
        identity.AddClaim(new Claim(ClaimTypes.Name,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email)?.Value ?? userData.Email));
        identity.AddClaims(jwt.Claims.Where(u => u.Type == "role").Select(role => new Claim(ClaimTypes.Role, role?.Value ?? SD.RoleCustomer)));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegistrationDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationDTO registration)
    {
        if (ModelState.IsValid)
        {
            var registrationResult = await _authService.RegisterAsync(registration);
            if (registrationResult != null)
            {
                if (registrationResult.IsSuccess)
                {
                    //assign default customer role
                    registration.RoleName = SD.Roles.CUSTOMER.ToString();
                    var roleAssignResult = await _authService.AssignRoleAsync(registration);
                    if (roleAssignResult != null && roleAssignResult.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        registrationResult.IsSuccess = false;
                        TempData["error"] = registrationResult.Message;
                    }
                }
                else
                {
                    registrationResult.IsSuccess = false;
                    TempData["error"] = registrationResult.Message;
                }
            }
            else TempData["error"] = "Internal error occured";
        }

        registration.Password = registration.ConfirmPassword = "";
        return View(registration);
    }
}