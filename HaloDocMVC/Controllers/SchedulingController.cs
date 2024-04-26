using DocumentFormat.OpenXml.Spreadsheet;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace HaloDocMVC.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly HaloDocContext _context;
        private readonly IDropdown _dropdown;
        private readonly IScheduling _scheduling;
        public SchedulingController(HaloDocContext context, IDropdown dropdown, IScheduling scheduling)
        {
            _context = context;
            _dropdown = dropdown;
            _scheduling = scheduling;
        }

        #region SchedulingIndex
        public async Task<IActionResult> Index(int? region)
        {
            ViewBag.RegionComboBox = _dropdown.AllRegion();
            int id = Int32.Parse(CredentialValue.UserId());
            ViewBag.PhysiciansByRegion = _dropdown.PhysiciansByRegion(id);
            SchedulingData modal = new SchedulingData();
            if (modal == null)
            {
                return NotFound();
            }
            return View("../Scheduling/Index", modal);

        }
        #endregion

        #region GetPhysicianByRegion
        public IActionResult GetPhysicianByRegion(int regionid)
        {
            var PhysiciansByRegion = _dropdown.ProviderByRegion(regionid);
            return Json(PhysiciansByRegion);
        }
        #endregion

        #region LoadSchedulingPartial
        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            switch (PartialName)
            {
                case "_DayWise":
                    return PartialView("../Scheduling/_DayWise", _scheduling.Daywise(regionid, currentDate));

                case "_WeekWise":
                    return PartialView("../Scheduling/_WeekWise", _scheduling.Weekwise(regionid, currentDate));

                case "_MonthWise":
                    return PartialView("../Scheduling/_MonthWise", _scheduling.Monthwise(regionid, currentDate));

                default:
                    return PartialView("../Scheduling/_MonthWise", _scheduling.Monthwise(regionid, currentDate));
            }
        }
        #endregion

        #region LoadSchedulingPartialProivder
        public IActionResult LoadSchedulingPartialProivder(string date)
        {
            var currentDate = DateTime.Parse(date);
            return PartialView("../Scheduling/_MonthWise", _scheduling.MonthwisePhysician(currentDate, Int32.Parse(CredentialValue.UserId())));
        }
        #endregion

        #region AddShift
        public IActionResult AddShift(SchedulingData model)
        {
            string adminId = CredentialValue.ID();
            var chk = Request.Form["repeatdays"].ToList();
            if(CredentialValue.role() == "Provider")
            {
                model.regionid = _context.PhysicianRegions.Where(r => r.PhysicianId == Int32.Parse(CredentialValue.UserId())).Select(r => r.RegionId).FirstOrDefault();
            }
            _scheduling.AddShift(model, chk, adminId);
            return RedirectToAction("Index");
        }
        #endregion

        #region EditShift
        public SchedulingData viewshift(int shiftdetailid)
        {
            SchedulingData modal = _scheduling.ViewShift(shiftdetailid);
            return modal;
        }

        public IActionResult ViewShiftreturn(SchedulingData modal)
        {
            _scheduling.ViewShiftreturn(modal);
            return RedirectToAction("Index");
        }

        public void ViewShiftSave(SchedulingData modal)
        {
            _scheduling.ViewShiftSave(modal, CredentialValue.ID());
        }

        public IActionResult ViewShiftDelete(SchedulingData modal)
        {
            _scheduling.ViewShiftDelete(modal, CredentialValue.ID());
            return RedirectToAction("Index");
        }
        #endregion

        #region ProviderOnCall
        public async Task<IActionResult> MDSOnCall(int? regionId)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            List<ProviderMenu> v = await _scheduling.PhysicianOnCall(regionId);
            if (v == null)
            {
                return NotFound();
            }
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../Scheduling/MDsOnCall", v);
        }
        #endregion

        #region RequestedShift
        public IActionResult RequestedShift(int? regionId, SchedulingData sd)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            SchedulingData v = _scheduling.GetAllNotApprovedShift(regionId, sd);
            if (v == null)
            {
                return NotFound();
            }
            return View("../Scheduling/ReviewShift", v);
        }
        #endregion

        #region _ApprovedShifts
        public async Task<IActionResult> _ApprovedShifts(string shiftids)
        {
            if (await _scheduling.UpdateStatusShift(shiftids, CredentialValue.ID()))
            {
                TempData["Status"] = "Approved Shifts Successfully..!";
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion

        #region _DeleteShifts
        public async Task<IActionResult> _DeleteShifts(string shiftids)
        {
            if (await _scheduling.DeleteShift(shiftids, CredentialValue.ID()))
            {
                TempData["Status"] = "Delete Shifts Successfully..!";
            }
            return RedirectToAction("RequestedShift");
        }
        #endregion
    }
}
