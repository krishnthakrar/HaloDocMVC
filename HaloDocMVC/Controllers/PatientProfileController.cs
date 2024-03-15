using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
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
            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            var UserProfile = _dashboard.UserProfile(id);
            return View(UserProfile);
        }

        [HttpPost]
        public IActionResult Index(ViewDataUserProfile vdup)
        {
            int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
            _dashboard.EditProfile(vdup, id);
            return RedirectToAction("Index", "PatientProfile");
        }
    }
}
