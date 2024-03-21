using HaloDocMVC.Entity.DataContext;
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

        public PatientRequestController(HaloDocContext context, IPatientRequest patientrequest, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _patientrequest = patientrequest;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePatient(ViewDataCreatePatient vdcp)
        {
            _patientrequest.CreatePatient(vdcp);
            return View("../PatientHome/RequestLanding");
        }

        public IActionResult CreateFriend()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFriend(ViewDataCreateFriend vdcf)
        {
            _patientrequest.CreateFriend(vdcf);
            return View("../PatientHome/RequestLanding");
        }

        public IActionResult CreateConcierge()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateConcierge(ViewDataCreateConcierge vdcc)
        {
            _patientrequest.CreateConcierge(vdcc);
            return View("../PatientHome/RequestLanding");
        }

        public IActionResult CreatePartner()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePartner(ViewDataCreateBusiness vdcb)
        {
            _patientrequest.CreatePartner(vdcb);
            return View("../PatientHome/RequestLanding");
        }

        public IActionResult CreateMe()
        {
            int id = Int32.Parse(CredentialValue.UserId());
            var ViewMe = _patientrequest.ViewMe(id);
            return View(ViewMe);
        }

        [HttpPost]
        public IActionResult CreateMe(ViewDataCreatePatient vdcp)
        {
            _patientrequest.CreateMe(vdcp);
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult CreateSomeOneElse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSomeOneElse(ViewDataCreateSomeOneElse vdcs)
        {
            _patientrequest.CreateSomeOneElse(vdcs);
            return RedirectToAction("Index", "Dashboard");
        }

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

    }
}
