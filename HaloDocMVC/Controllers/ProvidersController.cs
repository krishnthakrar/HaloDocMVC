using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class ProvidersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditProvider()
        {
            return View();
        }
    }
}
