using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HaloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestRepository _irequestRepository;
        private readonly IViewCase _iviewcase;
        public HomeController(IRequestRepository irequestRepository, IViewCase iviewcase)
        {
            _irequestRepository = irequestRepository;
            _iviewcase = iviewcase;
        }
        public IActionResult Index()
        {
            CountStatusWiseRequestModel cswr = _irequestRepository.IndexData();
            return View(cswr);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ViewCase(int? id, int? id2)
        {
            ViewDataViewCase vdvc = _iviewcase.NewRequestData(id, id2);
            return View(vdvc);
        }
        public IActionResult ViewNotes(int? id)
        {
            return View();
        }
    }
}