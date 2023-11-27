using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
