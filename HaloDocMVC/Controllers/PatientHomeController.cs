using HaloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HaloDocMVC.Controllers
{
    public class PatientHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RequestLanding()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
