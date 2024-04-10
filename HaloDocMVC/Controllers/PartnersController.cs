using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class PartnersController : Controller
    {
        private readonly IDropdown _dropdown;
        private readonly IPartners _partners;
        private readonly INotyfService _notyf;
        public PartnersController(IDropdown idropdown, IPartners partners, INotyfService notyf)
        {
            _dropdown = idropdown;
            _partners = partners;
            _notyf = notyf;
        }

        #region Vendors
        public IActionResult Index(string searchValue, int Profession)
        {
            ViewBag.Profession = _dropdown.HealthProfessionalType();
            List<PartnersData> data = _partners.GetPartnersByProfession(searchValue, Profession);
            return View("../Partners/Index", data);
        }
        #endregion

        #region AddBusiness
        public IActionResult AddBusiness()
        {
            ViewBag.Profession = _dropdown.HealthProfessionalType();
            return View();
        }

        [HttpPost]
        public IActionResult AddBusiness(PartnersData pd)
        {
            if (_partners.AddBusiness(pd) == true)
            {
                _notyf.Success("Business Added successfully...");
            }
            else
            {
                _notyf.Error("Business cant be added...");
            }
            return RedirectToAction("Index", "Partners");
        }
        #endregion
    }
}
