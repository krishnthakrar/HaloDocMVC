using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class ProviderLocationController : Controller
    {
        [AdminAccess]
        [ProviderAccess]
        public IActionResult Index()
        {
            return View();
        }
    }
}
