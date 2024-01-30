using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MT.Web.Controllers;

[Authorize]
public class AccountController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}