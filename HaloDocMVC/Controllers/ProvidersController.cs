﻿using AspNetCoreHero.ToastNotification.Abstractions;
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
        public IActionResult ProviderInfoEdit(ProviderMenu pm, IFormFile? file, IFormFile? file1)
        {
            if (_providers.ProviderInfoEdit(pm, file, file1))
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
    }
}
