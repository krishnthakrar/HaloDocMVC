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
        public ButtonsController(IButtons buttons)
        {
            _buttons = buttons;
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
    }
}
