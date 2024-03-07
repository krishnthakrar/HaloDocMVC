using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace HaloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestRepository _irequestRepository;
        private readonly IAdminDashboardActions _admindashboardactions;
        private readonly IDropdown _dropdown;
        private readonly INotyfService _notyf;
        public HomeController(IRequestRepository irequestRepository, IAdminDashboardActions admindashboardactions, IDropdown idropdown, INotyfService notyf)
        {
            _irequestRepository = irequestRepository;
            _admindashboardactions = admindashboardactions;
            _dropdown = idropdown;
            _notyf = notyf;
        }

        [AdminAccess]
        public IActionResult Index()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewBag.CaseReason = _dropdown.CaseReason();
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

        public IActionResult ViewCase(int? RId, int? RTId, int? Status)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewDataViewCase vdvc = _admindashboardactions.NewRequestData(RId, RTId, Status);
            return View(vdvc);
        }

        [HttpPost]
        public IActionResult ViewCase(ViewDataViewCase vdvc, int? RId, int? RTId, int? Status)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewDataViewCase vc = _admindashboardactions.Edit(vdvc, RId, RTId, Status);
            return View(vc);
        }
        public IActionResult ViewNotes(int RId)
        {
            ViewDataViewNotes vdvn = _admindashboardactions.GetNotesByID(RId);
            return View("../Home/ViewNotes", vdvn);
        }

        public IActionResult ChangeNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _admindashboardactions.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Note Updated");
                    return View("../Home/ViewNotes");
                }
            }
            else
            {
                _notyf.Information("Please Select one of the note!!");
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
        }

        public IActionResult ProviderByRegion(int Regionid)
        {
            var v = _dropdown.ProviderByRegion(Regionid);
            return Json(v);
        }

        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _admindashboardactions.AssignProvider(requestid, ProviderId, Notes))
            {
                _notyf.Success("Physician Assigned successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Assigned...");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {
            bool CancelCase = _admindashboardactions.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _notyf.Success("Case Canceled Successfully");

            }
            else
            {
                _notyf.Error("Case Not Canceled");

            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _admindashboardactions.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                _notyf.Success("Case Blocked Successfully");
            }
            else
            {
                _notyf.Error("Case Not Blocked");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ClearCase(int RequestID)
        {
            bool cc = _admindashboardactions.ClearCase(RequestID);
            if (cc)
            {
                _notyf.Success("Case Cleared...");
                _notyf.Warning("You can not show Cleared Case ...");
            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Home", new { Status = "2" });
        }

        public IActionResult AuthError()
        {
            return View("../Home/AuthError");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}