using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
