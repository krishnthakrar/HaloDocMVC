using HaloDocMVC.Entity.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HaloDocMVC.Controllers
{
    public class PatientLoginController : Controller
    {
        private readonly HaloDocContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PatientLoginController(HaloDocContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Email, string PasswordHash)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == PasswordHash);

            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../PatientLogin/Index");
            }
            else
            {
                int id = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id).UserId;
                string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();

                _httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                _httpContextAccessor.HttpContext.Session.SetString("Name", userName);

                return RedirectToAction("Index", "Dashboard");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
