using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Controllers
{
    public class ButtonsController : Controller
    {
        private readonly IButtons _buttons;
        private readonly INotyfService _notyf;
        public ButtonsController(IButtons buttons, INotyfService notyf)
        {
            _buttons = buttons;
            _notyf = notyf;
        }

        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRequest(ViewDataCreatePatient vdcp)
        {
            _buttons.CreateRequest(vdcp);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SendLink(string FirstName, string LastName, string Email)
        {
            if (_buttons.SendLink(FirstName, LastName, Email))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
