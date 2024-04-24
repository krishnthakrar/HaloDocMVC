﻿using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Controllers
{
    public class PatientRequestController : Controller
    {

        private readonly HaloDocContext _context;
        private readonly IPatientRequest _patientrequest;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDropdown _dropdown;
        public PatientRequestController(HaloDocContext context, IPatientRequest patientrequest, IHttpContextAccessor httpContextAccessor, IDropdown idropdown)
        {
            _context = context;
            _patientrequest = patientrequest;
            _httpContextAccessor = httpContextAccessor;
            _dropdown = idropdown;
        }
        #region CreatePatient
        public IActionResult CreatePatient()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }

        [HttpPost]
        public IActionResult CreatePatient(ViewDataCreatePatient vdcp)
        {
            _patientrequest.CreatePatient(vdcp);
            return View("../PatientHome/RequestLanding");
        }
        #endregion

        #region CreateFriend
        public IActionResult CreateFriend()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }

        [HttpPost]
        public IActionResult CreateFriend(ViewDataCreateFriend vdcf)
        {
            _patientrequest.CreateFriend(vdcf);
            return View("../PatientHome/RequestLanding");
        }
        #endregion

        #region CreateConcierge
        public IActionResult CreateConcierge()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }

        [HttpPost]
        public IActionResult CreateConcierge(ViewDataCreateConcierge vdcc)
        {
            _patientrequest.CreateConcierge(vdcc);
            return View("../PatientHome/RequestLanding");
        }
        #endregion

        #region CreatePartner
        public IActionResult CreatePartner()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }

        [HttpPost]
        public IActionResult CreatePartner(ViewDataCreateBusiness vdcb)
        {
            _patientrequest.CreatePartner(vdcb);
            return View("../PatientHome/RequestLanding");
        }
        #endregion

        #region CreateMe
        public IActionResult CreateMe()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            int id = Int32.Parse(CredentialValue.UserId());
            var ViewMe = _patientrequest.ViewMe(id);
            if (ViewMe == null)
            {
                return NotFound();
            }
            return View(ViewMe);
        }

        [HttpPost]
        public IActionResult CreateMe(ViewDataCreatePatient vdcp)
        {
            _patientrequest.CreateMe(vdcp);
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region CreateSomeOneElse
        public IActionResult CreateSomeOneElse()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            return View();
        }

        [HttpPost]
        public IActionResult CreateSomeOneElse(ViewDataCreateSomeOneElse vdcs)
        {
            _patientrequest.CreateSomeOneElse(vdcs);
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region CheckEmailAsync
        [HttpPost]
        public async Task<IActionResult> CheckEmailAsync(string email)
        {
            string message;
            var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
            if (aspnetuser == null)
            {
                message = "False";

            }
            else
            {
                message = "success";

            }
            return Json(new
            {
                Message = message,
            });
        }
        #endregion
    }
}
