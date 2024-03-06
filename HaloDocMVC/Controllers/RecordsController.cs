using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class RecordsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
