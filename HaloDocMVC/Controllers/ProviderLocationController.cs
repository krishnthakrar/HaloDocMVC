using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class ProviderLocationController : Controller
    {
        private readonly IProviderLoc _ProviderLoc;
        private readonly INotyfService _notyf;

        public ProviderLocationController(IProviderLoc ProviderLoc,
                                      INotyfService notyf)
        {
            _ProviderLoc = ProviderLoc;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            ViewBag.Log = _ProviderLoc.FindPhysicianLocation();
            return View("../ProviderLocation/Index");
        }
    }
}
