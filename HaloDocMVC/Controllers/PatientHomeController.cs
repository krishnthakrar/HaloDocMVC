using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HaloDocMVC.Controllers
{
    public class PatientHomeController : Controller
    {
        private readonly ILogin _login;
        private readonly INotyfService _notyf;

        public PatientHomeController(INotyfService notyf, ILogin login)
        {
            _notyf = notyf;
            _login = login;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RequestLanding()
        {
            return View();
        }

        #region CreateAccountPost
        [HttpPost]
        public IActionResult CreateAccountPost(string Email, string Password)
        {
            if (_login.CreateAccount(Email, Password))
            {
                _notyf.Success("User Created Successfully..............");
            }
            else
            {
                _notyf.Error("User cant be created............");
            }
            return RedirectToAction("Index", "PatientLogin");
        }
        #endregion

        #region CreateAccount
        public IActionResult CreateAccount()
        {
            return View("../PatientHome/CreateAccount");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
