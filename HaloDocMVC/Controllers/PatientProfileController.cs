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

        public PatientProfileController(IHttpContextAccessor httpContextAccessor, IDashboard dashboard)
        {
            _httpContextAccessor = httpContextAccessor;
            _dashboard = dashboard;
        }
        public IActionResult Index()
        {
            int id = Int32.Parse(CredentialValue.UserId());
            var UserProfile = _dashboard.UserProfile(id);
            return View(UserProfile);
        }

        [HttpPost]
        public IActionResult Index(ViewDataUserProfile vdup)
        {
            int id = Int32.Parse(CredentialValue.UserId());
            _dashboard.EditProfile(vdup, id);
            return RedirectToAction("Index", "PatientProfile");
        }
    }
}
