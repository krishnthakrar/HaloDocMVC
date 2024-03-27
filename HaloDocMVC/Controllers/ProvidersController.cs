using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HaloDocMVC.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly IDropdown _dropdown;
        private readonly IProviders _providers;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly INotyfService _notyf;
        public ProvidersController(IDropdown idropdown, IProviders providers, EmailConfiguration emailConfiguration, INotyfService notyf)
        {
            _dropdown = idropdown;
            _providers = providers;
            _emailConfiguration = emailConfiguration;
            _notyf = notyf;
        }
        public IActionResult Index(int? region)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            if (region == null)
            {
                var v = _providers.PhysicianAll();
                return View("../Providers/Index", v);
            }
            else
            {
                var v = _providers.PhysicianByRegion(region);
                return View("../Providers/Index", v);
            }
        }

        #region ChangeNotificationPhysician
        public IActionResult ChangeNotificationPhysician(string changedValues)
        {
            Dictionary<int, bool> changedValuesDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _providers.ChangeNotificationPhysician(changedValuesDict);
            return RedirectToAction("Index");
        }
        #endregion

        #region SendMessage
        public async Task<IActionResult> SendMessage(string? email, int? contactProviderRadioSet, string? Message)
        {
            bool result;
            if (contactProviderRadioSet == 1)
            {
                result = false;
            }
            else if (contactProviderRadioSet == 2)
            {
                result = await _emailConfiguration.SendMail(email, "Mail from Admin", Message);
            }
            else
            {
                result = await _emailConfiguration.SendMail(email, "Mail from Admin", Message);
            }
            if (result == true)
            {
                _notyf.Success("Mail Sent Successfully..!");
            }
            else
            {
                _notyf.Success("Message Sent Successfully..!");
            }
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult EditProvider()
        {
            return View();
        }
    }
}
