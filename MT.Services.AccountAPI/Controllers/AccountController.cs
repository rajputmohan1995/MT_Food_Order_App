using Microsoft.AspNetCore.Mvc;

namespace MT.Services.AccountAPI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
