using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class PartnersController : Controller
    {
        private readonly IDropdown _dropdown;
        private readonly IPartners _partners;
        public PartnersController(IDropdown idropdown, IPartners partners)
        {
            _dropdown = idropdown;
            _partners = partners;
        }
        public IActionResult Index(string searchValue, int Profession)
        {
            ViewBag.Profession = _dropdown.HealthProfessionalType();
            List<PartnersData> data = _partners.GetPartnersByProfession(searchValue, Profession);
            return View("../Partners/Index", data);
        }
    }
}
