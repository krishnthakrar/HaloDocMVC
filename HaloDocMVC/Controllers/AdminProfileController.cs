using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class AdminProfileController : Controller
    {
        private readonly IAdminProfile _adminprofile;
        private readonly IDropdown _dropdown;
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        public AdminProfileController(ILogger<HomeController> logger, IAdminProfile adminprofile, IDropdown dropdown, INotyfService INotyfService)
        {
            _adminprofile = adminprofile;
            _dropdown = dropdown;
            _logger = logger;
            _notyf = INotyfService;
        }

        #region Index
        public IActionResult Index(int? id)
        {
            ViewAdminProfile p = _adminprofile.GetProfileDetails((id != null ? (int)id : Convert.ToInt32(CredentialValue.UserId())));
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewBag.UserRole = _dropdown.UserRole();
            return View("../AdminProfile/Index", p);
        }
        #endregion

        #region EditPassword
        public IActionResult EditPassword(string password)
        {
            if (_adminprofile.EditPassword(password, Convert.ToInt32(CredentialValue.UserId())))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public IActionResult EditAdministratorInfo(ViewAdminProfile _viewAdminProfile)
        {
            if (_adminprofile.EditAdministratorInfo(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public IActionResult BillingInfoEdit(ViewAdminProfile _viewAdminProfile)
        {
            if (_adminprofile.BillingInfoEdit(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
