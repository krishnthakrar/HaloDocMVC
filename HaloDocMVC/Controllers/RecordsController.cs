using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
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
    }
}
