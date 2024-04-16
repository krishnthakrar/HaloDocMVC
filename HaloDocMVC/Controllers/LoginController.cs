using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAdminDashboardActions _AdminDashBoardActions;
        private readonly IDropdown _dropdown;
        private readonly INotyfService _notyf;
        private readonly ILogger<HomeController> _logger;
        private readonly ILogin _login;
        private readonly IJwt _jwt;
        private readonly HaloDocContext _context;
        public LoginController(ILogger<HomeController> logger,
                                      IDropdown dropdown,
                                      IAdminDashboardActions AdminDashBoardActions,
                                      INotyfService notyf,
                                      ILogin login,
                                      IJwt jwt,
                                      HaloDocContext context)
        {
            _dropdown = dropdown;
            _AdminDashBoardActions = AdminDashBoardActions;
            _notyf = notyf;
            _logger = logger;
            _login = login;
            _jwt = jwt;
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../Login/Index");
        }

        #region Validate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(AspNetUser aspNetUser)
        {
            UserInfo u = await _login.CheckAccessLogin(aspNetUser);
            
            if (u != null)
            {
                var jwttoken = _jwt.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Status", "1");
                if (u.Role == "Patient")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (u.Role == "Provider")
                {
                    return Redirect("~/Physician/DashBoard");
                }
                return Redirect("~/Admin/DashBoard");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../Login/Index");
            }
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Login");
        }
        #endregion

        #region AuthError
        public IActionResult AuthError()
        {
            return View("../Home/AuthError");
        }
        #endregion

        #region ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }
        #endregion

        #region ResetPassword
        public IActionResult ResetEmail(string Email)
        {
            if (_login.SendResetLink(Email))
            {
                _notyf.Success("Mail Send Successfully..!");
            }
            return RedirectToAction("ForgotPassword", "Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string datetime)
        {
            TempData["email"] = email;
            TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
            if (time.TotalHours > 24)
            {
                return View("LinkExpired");
            }
            else
            {
                return View();
            }
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
                return RedirectToAction("Index", "Login");
            }
            else
            {
                TempData["emailmessage"] = "Email is not registered!!";
                return View("ResetPassword");
            }
        }
        #endregion

    }
}
