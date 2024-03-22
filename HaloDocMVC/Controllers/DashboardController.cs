using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HaloDocContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDashboard _dashboard;
        public DashboardController(HaloDocContext context, IHttpContextAccessor httpContextAccessor, IDashboard dashboard)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dashboard = dashboard;
        }
        public IActionResult Index()
        {
            int id = Int32.Parse(CredentialValue.UserId());
            var result = _dashboard.DashboardList(id);
            return View(result);
        }

        #region ViewDoc
        public IActionResult ViewDoc(int id)
        {
            var DocList = _dashboard.ViewDocumentList(id);
            return View(DocList);
        }
        #endregion

        #region ViewDocPost
        [HttpPost]
        public IActionResult ViewDoc(int id, IFormFile? UploadFile)
        {
            _dashboard.UploadDoc(id, UploadFile);
            return RedirectToAction("ViewDoc", "Dashboard");
        }
        #endregion
    }
}
