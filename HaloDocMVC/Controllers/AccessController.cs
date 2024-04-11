using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class AccessController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly IAccess _access;
        private readonly IDropdown _dropdown;
        private readonly IProviders _providers;
        private readonly IAdminProfile _adminprofile;
        public AccessController(INotyfService notyf, IAccess access, IDropdown dropdown, IProviders providers, IAdminProfile adminprofile)
        {
            _notyf = notyf;
            _access = access;
            _dropdown = dropdown;
            _providers = providers;
            _adminprofile = adminprofile;
        }

        #region Index
        public IActionResult Index()
        {
            var v = _access.AccessIndex();
            return View("../Access/Index", v);
        }
        #endregion

        #region CreateAccess
        public IActionResult CreateAccess()
        {
            ViewBag.AccType = _dropdown.AccType();
            return View();
        }
        #endregion

        #region CreateAccessPost
        public IActionResult CreateAccessPost(AccessMenu am)
        {
            string id = CredentialValue.ID();
            if (_access.CreateAccessPost(am, id))
            {
                _notyf.Success("Role added Successfully...");
            }
            else
            {
                _notyf.Error("Role can't be added...");
            }
            return RedirectToAction("Index", "Access");
        }
        #endregion

        #region CreateAccessDropDown
        public Task<IActionResult> AccessByType(int AccountType)
        {
            var v = _dropdown.AccessByType(AccountType);
            return Task.FromResult<IActionResult>(Json(v));
        }
        #endregion

        #region DeleteAccess
        public IActionResult DeleteAccess(int? id)
        {
            if (_access.DeleteAccess(id))
            {
                _notyf.Success("Role Deleted Successfully...");
            }
            else
            {
                _notyf.Error("Role can't be Deleted...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAccess
        public IActionResult EditAccess(int? id, int AccountType)
        {
            ViewBag.AccType = _dropdown.AccType();
            ViewBag.AccessByType = _dropdown.AccessByType(AccountType);
            AccessMenu am = _access.EditAccess(id);
            return View(am);
        }
        #endregion

        #region EditAccessPost
        [HttpPost]
        public IActionResult EditAccessPost(AccessMenu am)
        {
            if (_access.EditAccessPost(am))
            {
                _notyf.Success("Role edited Successfully...");
            }
            else
            {
                _notyf.Error("Role can't be edited...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region UserAccess
        public IActionResult UserAccess(int? role)
        {
            List<UserAccess> data = _access.GetAllUserDetails(role);
            if (role != null)
            {
                data = _access.GetAllUserDetails(role);
            }
            return View("../Access/UserAccess", data);
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
            return RedirectToAction("UserAccess", "Access");
        }
        #endregion

        #region EditProvider
        public IActionResult EditProvider(int? id)
        {
            ViewBag.PhysRole = _dropdown.PhysRole();
            ViewBag.AllRegion = _dropdown.AllRegion();
            ProviderMenu p = _providers.GetProfileDetails((int)id);
            return View("../Access/EditProvider", p);
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
            return RedirectToAction("UserAccess");
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
            return RedirectToAction("UserAccess");
        }
        #endregion

        #region CreateAdmin
        public IActionResult CreateAdmin()
        {
            ViewBag.UserRole = _dropdown.PhysRole();
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }
        #endregion

        #region CreateAdminPost
        public IActionResult CreateAdminPost(ViewAdminProfile vap)
        {
            string? id = CredentialValue.ID();
            if (_access.CreateAdmin(vap, id))
            {
                _notyf.Success("Admin created successfully....!!!!");
            }
            else
            {
                _notyf.Error("Admin already exists.....!!!!!!");
            }
            return RedirectToAction("UserAccess", "Access");
        }
        #endregion

        #region EditAdmin
        public IActionResult EditAdmin(int? id)
        {
            ViewAdminProfile p = _adminprofile.GetProfileDetails((id != null ? (int)id : Convert.ToInt32(CredentialValue.UserId())));
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewBag.UserRole = _dropdown.UserRole();
            return View("../Access/EditAdmin", p);
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
            return RedirectToAction("EditAdmin");
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
            return RedirectToAction("EditAdmin");
        }
        #endregion

        #region EditBillingInfo
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
            return RedirectToAction("EditAdmin");
        }
        #endregion
    }
}
