using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HaloDocMVC.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly IDropdown _dropdown;
        private readonly IProviders _providers;
        public ProvidersController(IDropdown idropdown, IProviders providers)
        {
            _dropdown = idropdown;
            _providers = providers;
        }
        public IActionResult Index(int? region)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            var v = _providers.PhysicianAll();
            if (region == null)
            {
                v = _providers.PhysicianAll();
            }
            /*else
            {
                v = _providers.PhysicianByRegion(region);
            }*/
            return View("../Providers/Index", v);
        }

        #region ChangeNotificationPhysician
        public IActionResult ChangeNotificationPhysician(string changedValues)
        {
            Dictionary<int, bool> changedValuesDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _providers.ChangeNotificationPhysician(changedValuesDict);
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult EditProvider()
        {
            return View();
        }
    }
}
