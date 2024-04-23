using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Office2010.Excel;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
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
        [ProviderAccess("Admin")]
        public IActionResult Index(ProviderMenu pm, int? Region)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            if (Region == null)
            {
                ProviderMenu v = _providers.PhysicianAll(pm);
                return View("../Providers/Index", v);
            }
            else
            {
                ProviderMenu v = _providers.PhysicianByRegion(pm, Region);
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
        public async Task<IActionResult> SendMessage(string? email, string? mobile, int? contactProviderRadioSet, string? Message)
        {
            bool result = false;
            bool sms = false;
            if (contactProviderRadioSet == 1)
            {
                bool MessageLog = _providers.MessageLog(mobile, Message);
                sms = _providers.SendMessage(mobile, Message);
            }
            else if (contactProviderRadioSet == 2)
            {
                bool EmailLog = _providers.EmailLog(email, "Mail from Admin", Message);
                result = await _emailConfiguration.SendMail(email, "Mail from Admin", Message);
            }
            else
            {
                bool EmailLog = _providers.EmailLog(email, "Mail from Admin", Message);
                bool MessageLog = _providers.MessageLog(mobile, Message);
                result = await _emailConfiguration.SendMail(email, "Mail from Admin", Message);
                sms = _providers.SendMessage(mobile, Message);
            }
            if (result)
            {
                _notyf.Success("Mail Sent Successfully..!");
            }
            if (sms)
            {
                _notyf.Success("Message Sent Successfully..!");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateProvider
        public IActionResult CreateProvider()
        {
            ViewBag.PhysRole = _dropdown.PhysRole();
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }
        #endregion

        #region CreateProviderPost
        [HttpPost]
        public IActionResult CreateProviderPost(ProviderMenu pm)
        {
            string? id = CredentialValue.ID();
            if (_providers.CreateProvider(pm, id))
            {
                _notyf.Success("Provider created successfully....!!!!");
            }
            else
            {
                _notyf.Error("Provider already exists.....!!!!!!");
            }
            return RedirectToAction("Index", "Providers");
        }
        #endregion

        #region EditProvider
        public IActionResult EditProvider(int? id)
        {
            ViewBag.PhysRole = _dropdown.PhysRole();
            ViewBag.AllRegion = _dropdown.AllRegion();
            ProviderMenu p = _providers.GetProfileDetails((int)id);
            return View("../Providers/EditProvider", p);
        }
        #endregion

        #region EditPassword
        public IActionResult EditPassword(string password, ProviderMenu pm)
        {
            if (_providers.EditPassword(password, pm))
            {
                _notyf.Success("Account Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Account Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { id = pm.PhysicianId });
        }
        #endregion

        #region EditPhysInfo
        [HttpPost]
        public IActionResult EditPhysInfo(ProviderMenu pm)
        {
            if (_providers.EditPhysInfo(pm))
            {
                _notyf.Success("Physician Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Physician Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { id = pm.PhysicianId });
        }
        #endregion

        #region EditBillingInfo
        [HttpPost]
        public IActionResult BillingInfoEdit(ProviderMenu pm)
        {
            if (_providers.BillingInfoEdit(pm))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { id = pm.PhysicianId });
        }
        #endregion

        #region EditProviderInfo
        [HttpPost]
        public IActionResult ProviderInfoEdit(ProviderMenu pm)
        {
            if (_providers.ProviderInfoEdit(pm))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("EditProvider", new { id = pm.PhysicianId });
        }
        #endregion

        #region ProviderEditSubmit
        [HttpPost]
        public IActionResult ProviderEditSubmit(ProviderMenu pm)
        {
            if (_providers.ProviderEditSubmit(pm))
            {
                _notyf.Success("Information edited Successfully...");
            }
            else
            {
                _notyf.Error("Information not edited...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region DeleteAccount
        public IActionResult DeleteAccount(int? id)
        {
            if (_providers.DeleteAccount(id))
            {
                _notyf.Success("Account Deleted Successfully...");
            }
            else
            {
                _notyf.Error("Account can't be Deleted...");
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
