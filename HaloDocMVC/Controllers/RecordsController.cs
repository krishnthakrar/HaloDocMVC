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
            if (model == null)
            {
                return NotFound();
            }
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
            if (r == null)
            {
                return NotFound();
            }
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
            if (rm == null)
            {
                return NotFound();
            }
            return View("../Records/PatientHistory", rm);
        }
        #endregion

        #region PatientRecords
        public IActionResult PatientRecords(PaginatedViewModel data, int UserId)
        {
            var r = _records.PatientRecord(UserId, data);
            if (r == null)
            {
                return NotFound();
            }
            return View("../Records/PatientRecords", r);
        }
        #endregion

        #region EmailLogs
        public IActionResult EmailLogs(RecordsModel rm)
        {
            RecordsModel r = _records.GetFilteredEmailLogs(rm);
            if (r == null)
            {
                return NotFound();
            }
            return View("../Records/EmailLogs", r);
        }
        #endregion EmailLogs

        #region SMSLogs
        public IActionResult SMSLogs(RecordsModel rm)
        {
            RecordsModel r = _records.GetFilteredSMSLogs(rm);
            if (r == null)
            {
                return NotFound();
            }
            return PartialView("../Records/SMSLogs", r);
        }
        #endregion
    }
}
