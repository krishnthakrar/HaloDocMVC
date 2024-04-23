using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class InvoicingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
