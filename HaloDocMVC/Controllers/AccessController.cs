using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
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

        public IActionResult Index()
        {
            var v = _access.AccessIndex();
            return View("../Access/Index", v);
        }

        public IActionResult CreateAccess()
        {
            ViewBag.AccType = _dropdown.AccType();
            return View();
        }

        public Task<IActionResult> AccessByType(int AccountType)
        {
            var v = _dropdown.AccessByType(AccountType);
            return Task.FromResult<IActionResult>(Json(v));
        }
    }
}
