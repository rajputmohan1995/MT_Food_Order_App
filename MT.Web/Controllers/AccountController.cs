using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MT.Web.Models;
using MT.Web.Service.Interface;
using Newtonsoft.Json;

namespace MT.Web.Controllers;

[Authorize]
public class AccountController : BaseController
{
    readonly IAuthService _authService;
    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        
        var userDetailModel = new UserDTO();
        var userDetailResponse = await _authService.GetUserDetailsAsync(GetLoggedInUserId());
        if (userDetailResponse != null && userDetailResponse.IsSuccess)
            userDetailModel = JsonConvert.DeserializeObject<UserDTO>(userDetailResponse.Result.ToString());
        return View(userDetailModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(UserDTO user)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var saveUserResponse = await _authService.SaveUserDetailsAsync(user);
                if (saveUserResponse != null)
                {
                    if (saveUserResponse.IsSuccess)
                        TempData["success"] = "User details saved successfully";
                    else TempData["error"] = saveUserResponse.Message;
                }
                else TempData["error"] = "Internal error occured while saving user details, please try again.";
            }
            else return View(user);
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("Index", "Account");
    }

    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordDTO() { UserID = GetLoggedInUserId() });
    }


    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePassword)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var changePasswordResponse = await _authService.ChangePasswordAsync(changePassword);
                if (changePasswordResponse != null)
                {
                    if (changePasswordResponse.IsSuccess)
                        TempData["success"] = "Login password reset successfully";
                    else TempData["error"] = changePasswordResponse.Message;
                }
                else TempData["error"] = "Internal error occured while resetting login password, please try again.";
            }
            else return View();
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
        }
        return RedirectToAction("ChangePassword", "Account");
    }
}