using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class AdminProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
