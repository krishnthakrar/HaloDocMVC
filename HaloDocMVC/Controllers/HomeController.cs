using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
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

        public async Task<IActionResult> _SearchResult(string Status)
        {
            List<AdminDashboardList> contacts = _irequestRepository.GetRequests(Status);

            switch (Status)
            {
                case "1":
                    return PartialView("../Home/NewState", contacts);
                case "2":
                    return PartialView("../Home/PendingState", contacts);
                case "4,5":
                    return PartialView("../Home/ActiveState", contacts);
                case "6":
                    return PartialView("../Home/ConcludeState", contacts);
                case "3,7,8":
                    return PartialView("../Home/ToCloseState", contacts);
                case "9":
                    return PartialView("../Home/UnPaidState", contacts);
                default:
                    break;
            }


            return PartialView("../Home/NewState", contacts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ViewCase(int? RId, int? RTId)
        {
            ViewDataViewCase vdvc = _iviewcase.NewRequestData(RId, RTId);
            return View(vdvc);
        }

        public IActionResult ViewNotes(int? id)
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