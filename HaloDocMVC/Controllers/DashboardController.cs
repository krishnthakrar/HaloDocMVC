using AspNetCoreHero.ToastNotification.Abstractions;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HaloDocMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HaloDocContext _context;
        private readonly IDashboard _dashboard;
        private readonly INotyfService _notyf;
        public DashboardController(HaloDocContext context, INotyfService notyf, IDashboard dashboard)
        {
            _context = context;
            _notyf = notyf;
            _dashboard = dashboard;
        }
        #region Dashboard
        public IActionResult Index(DashboardList listdata)
        {
            DashboardList data = _dashboard.GetPatientRequest(CredentialValue.UserId(), listdata);
            return View("../Dashboard/Index", data);
        }
        #endregion

        #region ViewDoc
        public async Task<IActionResult> ViewDoc(int? id, ViewDocument viewDocument)
        {
            if (id == null)
            {
                id = viewDocument.RequestId;
            }
            ViewDocument v = await _dashboard.ViewDocumentList(id, viewDocument);
            return View("../Dashboard/ViewDoc", v);
        }
        #endregion
    }
}
