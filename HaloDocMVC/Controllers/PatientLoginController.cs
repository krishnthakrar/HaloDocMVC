using AspNetCoreHero.ToastNotification.Abstractions;
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
        private readonly INotyfService _notyf;
        public PatientLoginController(HaloDocContext context, IJwt jwt, ILogin login, INotyfService notyf)
        {
            _context = context;
            _jwt = jwt;
            _login = login;
            _notyf = notyf;
        }
        public IActionResult Index()
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetEmail(string Email)
        {
            if (_login.SendResetLinkPatient(Email))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("ForgotPassword", "PatientLogin");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string datetime)
        {
            TempData["email"] = email;
            /*TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
            if (time.TotalHours > 24)
            {
                return View("LinkExpired");
            }
            else
            {
                return View();
            }*/
            return View();
        }
        [HttpPost]
        public IActionResult SavePassword(ViewDataCreatePatient viewPatientReq)
        {
            var aspnetuser = _context.AspNetUsers.FirstOrDefault(m => m.Email == viewPatientReq.Email);
            if (aspnetuser != null)
            {
                aspnetuser.PasswordHash = viewPatientReq.PasswordHash;
                _context.AspNetUsers.Update(aspnetuser);
                _context.SaveChanges();

                TempData["emailmessage"] = "Your password is changed!!";
                return RedirectToAction("Index", "PatientLogin");
            }
            else
            {
                TempData["emailmessage"] = "Email is not registered!!";
                return View("ResetPassword");
            }
        }
    }
}
