using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Controllers
{
    public class PatientProfileController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDashboard _dashboard;
        private readonly IDropdown _dropdown;

        public PatientProfileController(IHttpContextAccessor httpContextAccessor, IDashboard dashboard, IDropdown dropdown)
        {
            _httpContextAccessor = httpContextAccessor;
            _dashboard = dashboard;
            _dropdown = dropdown;
        }

        #region Profile
        public IActionResult Index()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            int id = Int32.Parse(CredentialValue.UserId());
            if (id == null)
            {
                return NotFound();
            }
            var UserProfile = _dashboard.UserProfile(id);
            if (UserProfile == null)
            {
                return NotFound();
            }
            return View(UserProfile);
        }
        #endregion

        #region EditProfile
        [HttpPost]
        public IActionResult Index(ViewDataUserProfile vdup)
        {
            int id = Int32.Parse(CredentialValue.UserId());
            _dashboard.EditProfile(vdup, id);
            return RedirectToAction("Index", "PatientProfile");
        }
        #endregion
    }
}
