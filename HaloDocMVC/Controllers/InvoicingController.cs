using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class InvoicingController : Controller
    {
        private readonly IInvoiceInterface _invoice;
        private readonly INotyfService _notyf;

        public InvoicingController(IInvoiceInterface invoice, INotyfService notyf)
        {
            _invoice = invoice;
            _notyf = notyf;
        }
        [ProviderAccess("Admin,Provider")]
        [Route("Physician/Invoicing")]

        public IActionResult Index()
        {
            return View("../Invoicing/Index");
        }
        [Route("Invoicing")]
        public IActionResult IndexAdmin()
        {
            ViewBag.GetAllPhysicians = _invoice.GetAllPhysicians();
            return View("../Invoicing/IndexAdmin");
        }

        #region IsFinalizeSheet
        public IActionResult IsFinalizeSheet(int PhysicianId, DateOnly StartDate)
        {
            bool x = _invoice.isFinalizeTimesheet(PhysicianId, StartDate);
            return Json(new { x });
        }

        public IActionResult IsApproveSheet(int PhysicianId, DateOnly StartDate)
        {
            var x = _invoice.GetPendingTimesheet(PhysicianId, StartDate);
            if (x.Count() == 0)
            {
                return Json(new { x = true });
            }
            return PartialView("../Invoicing/_PendingApprove", x);
        }
        #endregion

        #region TimeSheetDetailsAddEdit_PageData
        public async Task<IActionResult> Timesheet(int PhysicianId, DateOnly StartDate)
        {
            if (CredentialValue.role() == "Provider" && _invoice.isFinalizeTimesheet(PhysicianId, StartDate))
            {
                _notyf.Error("Sheet Is Already Finalized");
                return RedirectToAction("Index");
            }
            int AfterDays = StartDate.Day == 1 ? 14 : DateTime.DaysInMonth(StartDate.Year, StartDate.Month) - 14; ;
            var TimeSheetDetails = _invoice.PostTimesheetDetails(PhysicianId, StartDate, AfterDays, CredentialValue.ID());
            List<ViewTimeSheetDetailReimbursementsdata> h = await _invoice.GetTimesheetBills(TimeSheetDetails);
            var Timesheet = _invoice.GetTimesheetDetails(TimeSheetDetails, h, PhysicianId);
            Timesheet.PhysicianId = PhysicianId;
            return View("../Invoicing/Timesheet", Timesheet);
        }
        #endregion

        public async Task<IActionResult> GetTimesheetDetailsData(int PhysicianId, DateOnly StartDate)
        {
            var Timesheet = new ViewTimeSheet();
            if (StartDate == DateOnly.MinValue)
            {
                Timesheet.ViewTimesheetDetails = new List<ViewTimeSheetDetails> { };
                Timesheet.ViewTimesheetDetailReimbursements = new List<ViewTimeSheetDetailReimbursementsdata> { };
            }
            else
            {
                List<TimeSheetDetail> x = _invoice.PostTimesheetDetails(PhysicianId, StartDate, 0, CredentialValue.ID());
                List<ViewTimeSheetDetailReimbursementsdata> h = await _invoice.GetTimesheetBills(x);
                Timesheet = _invoice.GetTimesheetDetails(x, h, PhysicianId);
            }
            if (Timesheet == null)
            {
                var Timesheets = new ViewTimeSheet();
                Timesheets.ViewTimesheetDetails = new List<ViewTimeSheetDetails> { };
                Timesheets.ViewTimesheetDetailReimbursements = new List<ViewTimeSheetDetailReimbursementsdata> { };
                return PartialView("../Invoicing/_TimesheetTable", Timesheets);
            }
            return PartialView("../Invoicing/_TimesheetTable", Timesheet);
        }
        public IActionResult TimeSheetDetailsEdit(ViewTimeSheet viewTimeSheet, int PhysicianId)
        {
            if (_invoice.PutTimesheetDetails(viewTimeSheet.ViewTimesheetDetails, CredentialValue.ID()))
            {
                _notyf.Success("TimeSheet Edited Successfully..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId, StartDate = viewTimeSheet.ViewTimesheetDetails[0].TimeSheetDate });
        }
        public IActionResult TimeSheetBillAddEdit(int? Trid, DateOnly Timesheetdate, IFormFile file, int Timesheetdetailid, int Amount, string Item, int PhysicianId, DateOnly StartDate)
        {
            ViewTimeSheetDetailReimbursementsdata timesheetdetailreimbursement = new ViewTimeSheetDetailReimbursementsdata();
            timesheetdetailreimbursement.TimeSheetDetailId = Timesheetdetailid;
            timesheetdetailreimbursement.TimeSheetDetailReimbursementId = Trid;
            timesheetdetailreimbursement.Amount = Amount;
            timesheetdetailreimbursement.BillFile = file;
            timesheetdetailreimbursement.ItemName = Item;
            if (_invoice.TimeSheetBillAddEdit(timesheetdetailreimbursement, CredentialValue.ID()))
            {
                _notyf.Success("Bill Changed Successfully..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #region TimeSheetBill_Delete
        public IActionResult TimeSheetBillRemove(int? Trid, int PhysicianId, DateOnly StartDate)
        {
            ViewTimeSheetDetailReimbursementsdata timesheetdetailreimbursement = new ViewTimeSheetDetailReimbursementsdata();
            timesheetdetailreimbursement.TimeSheetDetailReimbursementId = Trid;
            if (_invoice.TimeSheetBillRemove(timesheetdetailreimbursement, CredentialValue.ID()))
            {
                _notyf.Success("Bill deleted Successfully..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #endregion
        public IActionResult SetToFinalize(int timesheetid)
        {
            if (_invoice.SetToFinalize(timesheetid, CredentialValue.ID()))
            {
                _notyf.Success("Sheet Is Finalized Successfully..!");
            }
            return RedirectToAction("Index");
        }
        #region SetToApprove
        public async Task<IActionResult> SetToApprove(ViewTimeSheet ts)
        {
            if (await _invoice.SetToApprove(ts, CredentialValue.ID()))
            {
                _notyf.Success("Sheet Is Approved Successfully..!");
            }
            return RedirectToAction("IndexAdmin");
        }
        #endregion
    }
}
