using AspNetCoreHero.ToastNotification.Abstractions;
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
        public AccessController(INotyfService notyf, IAccess access, IDropdown dropdown)
        {
            _notyf = notyf;
            _access = access;
            _dropdown = dropdown;
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
    }
}
