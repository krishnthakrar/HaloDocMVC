using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
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
        public LoginController(ILogger<HomeController> logger,
                                      IDropdown dropdown,
                                      IAdminDashboardActions AdminDashBoardActions,
                                      INotyfService notyf,
                                      ILogin login,
                                      IJwt jwt)
        {
            _dropdown = dropdown;
            _AdminDashBoardActions = AdminDashBoardActions;
            _notyf = notyf;
            _logger = logger;
            _login = login;
            _jwt = jwt;
        }
        public IActionResult Index()
        {
            return View("../Login/Index");
        }
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../Login/Index");
            }
        }
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Login");
        }
        public IActionResult AuthError()
        {
            return View("../Home/AuthError");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

    }
}
