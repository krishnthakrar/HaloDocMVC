﻿using HaloDocMVC.Entity.DataContext;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDashboard _dashboard;
        public DashboardController(HaloDocContext context, IHttpContextAccessor httpContextAccessor, IDashboard dashboard)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dashboard = dashboard;
        }
        public IActionResult Index(DashboardList listdata)
        {
            DashboardList data = _dashboard.GetPatientRequest(CredentialValue.UserId(), listdata);
            return View("../Dashboard/Index", data);
        }

        #region ViewDoc
        public async Task<IActionResult> ViewDoc(int id, ViewDocument viewDocument)
        {
            if (id == null)
            {
                id = viewDocument.RequestId;
            }
            ViewDocument v = await _dashboard.ViewDocumentList(id, viewDocument);
            return View("../Dashboard/ViewDoc", v);
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
