using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HaloDocMVC.Controllers
{
    public class PatientLoginController : Controller
    {
        private readonly HaloDocContext _context;
        private readonly IJwt _jwt;
        private readonly ILogin _login;
        public PatientLoginController(HaloDocContext context, IJwt jwt, ILogin login)
        {
            _context = context;
            _jwt = jwt;
            _login = login;
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
        public async Task<IActionResult> Index(AspNetUser aspNetUser)
        {
            UserInfo u = await _login.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _jwt.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../PatientLogin/Index");
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index");
        }
    }
}
