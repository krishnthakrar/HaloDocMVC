using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class Agreement : Controller
    {
        private readonly IAdminDashboardActions _admindashboardactions;
        private readonly INotyfService _notyf;
        public Agreement(IAdminDashboardActions actionrepo, INotyfService notyf)
        {
            _admindashboardactions = actionrepo;
            _notyf = notyf;
        }
        public IActionResult Index(int RequestID, string PatientName)
        {
            TempData["RequestID"] = " " + RequestID;
            TempData["PatientName"] = "" + PatientName;

            return View();
        }
        public IActionResult accept(int RequestID)
        {
            _admindashboardactions.SendAgreement_accept(RequestID);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Reject(int RequestID, string Notes)
        {
            _admindashboardactions.SendAgreement_Reject(RequestID, Notes);
            return RedirectToAction("Index", "Home");
        }
    }
}
