using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class RecordsController : Controller
    {
        private readonly IRecords _records;
        private readonly INotyfService _notyf;
        public RecordsController(INotyfService iNotyfService, IRecords records)
        {
            _records = records;
            _notyf = iNotyfService;
        }

        #region SearchRecords
        public IActionResult Index(RecordsModel rm)
        {
            RecordsModel model = _records.GetFilteredSearchRecords(rm);
            return View("../Records/Index", model);
        }
        #endregion

        #region DeleteRequestSearchRecords
        public IActionResult DeleteRequest(int? RequestId)
        {
            if (_records.DeleteRequest(RequestId))
            {
                _notyf.Success("Request Deleted Successfully.");
            }
            else
            {
                _notyf.Error("Request not deleted");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region BlockHistory
        public IActionResult BlockedHistory(RecordsModel rm)
        {
            RecordsModel r = _records.BlockHistory(rm);
            return PartialView("../Records/BlockedHistory", r);
        }
        #endregion

        #region Unblock
        public IActionResult Unblock(int RequestId)
        {
            if (_records.Unblock(RequestId, CredentialValue.ID()))
            {
                _notyf.Success("Case Unblocked Successfully.");
            }
            else
            {
                _notyf.Error("Case remains blocked.");
            }

            return RedirectToAction("BlockedHistory");
        }
        #endregion

        #region PatientHistory
        public IActionResult PatientHistory(RecordsModel model)
        {
            RecordsModel rm = _records.GetFilteredPatientHistory(model);
            return View("../Records/PatientHistory", rm);
        }
        #endregion

        #region PatientRecords
        public IActionResult PatientRecords(PaginatedViewModel data, int UserId)
        {
            var r = _records.PatientRecord(UserId, data);
            return View("../Records/PatientRecords", r);
        }
        #endregion
    }
}
