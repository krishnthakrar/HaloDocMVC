using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Models;
using HaloDocMVC.Repository.Admin;
using HaloDocMVC.Repository.Admin.Repository;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreHero.ToastNotification.Abstractions;
using OfficeOpenXml;

namespace HaloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRequestRepository _irequestRepository;
        private readonly IAdminDashboardActions _admindashboardactions;
        private readonly IDropdown _dropdown;
        private readonly INotyfService _notyf;
        public HomeController(IRequestRepository irequestRepository, IAdminDashboardActions admindashboardactions, IDropdown idropdown, INotyfService notyf)
        {
            _irequestRepository = irequestRepository;
            _admindashboardactions = admindashboardactions;
            _dropdown = idropdown;
            _notyf = notyf;
        }

        [ProviderAccess("Admin")]
        public IActionResult Index()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewBag.CaseReason = _dropdown.CaseReason();
            PaginatedViewModel pvm = _irequestRepository.IndexData();
            return View(pvm);
        }

        #region SearchResult
        public async Task<IActionResult> _SearchResult(string Status, PaginatedViewModel data)
        {
            if (Status == null)
            {
                Status = CredentialValue.CurrentStatus();
            }
            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);

            PaginatedViewModel contacts = _irequestRepository.GetRequests(Status, data);

            switch (Status)
            {
                case "1":
                    return PartialView("../Home/NewState", contacts);
                case "2":
                    return PartialView("../Home/PendingState", contacts);
                case "4,5":
                    return PartialView("../Home/ActiveState", contacts);
                case "6":
                    return PartialView("../Home/ConcludeState", contacts);
                case "3,7,8":
                    return PartialView("../Home/ToCloseState", contacts);
                case "9":
                    return PartialView("../Home/UnPaidState", contacts);
                default:
                    break;
            }
            return PartialView("../Home/NewState", contacts);
        }
        #endregion

        #region ViewCase
        public IActionResult ViewCase(int? RId, int? RTId, int? Status)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewDataViewCase vdvc = _admindashboardactions.NewRequestData(RId, RTId, Status);
            return View(vdvc);
        }
        #endregion

        #region EditViewCase
        [HttpPost]
        public IActionResult ViewCase(ViewDataViewCase vdvc, int? RId, int? RTId, int? Status)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewDataViewCase vc = _admindashboardactions.Edit(vdvc, RId, RTId, Status);
            return View(vc);
        }
        #endregion

        #region ViewNotes
        public IActionResult ViewNotes(int RId)
        {
            ViewDataViewNotes vdvn = _admindashboardactions.GetNotesByID(RId);
            return View("../Home/ViewNotes", vdvn);
        }
        #endregion

        #region EditViewNotes
        public IActionResult ChangeNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _admindashboardactions.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    ViewDataViewNotes vdvn = _admindashboardactions.GetNotesByID(RequestID);
                    return View("../Home/ViewNotes", vdvn);
                }
                else
                {
                    _notyf.Error("Notes Not Updated");
                    return View("../Home/ViewNotes");
                }
            }
            else
            {
                _notyf.Information("Please Select one of the note!!");
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
        }
        #endregion

        #region ProviderByRegion
        public IActionResult ProviderByRegion(int Regionid)
        {
            var v = _dropdown.ProviderByRegion(Regionid);
            return Json(v);
        }
        #endregion

        #region AssignProvider
        public IActionResult AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (_admindashboardactions.AssignProvider(requestid, ProviderId, Notes) == true)
            {
                _notyf.Success("Physician Assigned successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Assigned...");
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region CancelCase
        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {
            bool CancelCase = _admindashboardactions.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _notyf.Success("Case Canceled Successfully");

            }
            else
            {
                _notyf.Error("Case Not Canceled");

            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region BlockCase
        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _admindashboardactions.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                _notyf.Success("Case Blocked Successfully");
            }
            else
            {
                _notyf.Error("Case Not Blocked");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region ClearCase
        public IActionResult ClearCase(int RequestID)
        {
            bool cc = _admindashboardactions.ClearCase(RequestID);
            if (cc)
            {
                _notyf.Success("Case Cleared...");
                _notyf.Warning("You can not show Cleared Case ...");
            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region TransferProvider
        public async Task<IActionResult> TransferProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _admindashboardactions.TransferProvider(requestid, ProviderId, Notes))
            {
                _notyf.Success("Physician Transfered successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Transfered...");
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region ViewUploadIndex
        public async Task<IActionResult> ViewUpload(int? id)
        {
            ViewDataViewDocuments v = await _admindashboardactions.GetDocumentByRequest(id);
            return View("../Home/ViewUpload", v);
        }
        #endregion

        #region UploadDoc
        public IActionResult UploadDoc(int Requestid, IFormFile file)
        {
            if (_admindashboardactions.SaveDoc(Requestid, file))
            {
                _notyf.Success("File Uploaded Successfully");
            }
            else
            {
                _notyf.Error("File Not Uploaded");
            }
            return RedirectToAction("ViewUpload", "Home", new { id = Requestid });
        }
        #endregion

        #region AllFilesDelete
        public async Task<IActionResult> AllFilesDelete(string deleteids, int Requestid)
        {
            if (await _admindashboardactions.DeleteDocumentByRequest(deleteids))
            {
                _notyf.Success("All Selected File Deleted Successfully");
            }
            else
            {
                _notyf.Error("All Selected File Not Deleted");
            }
            return RedirectToAction("ViewUpload", "Home", new { id = Requestid });
        }
        #endregion

        #region DeleteFile
        public async Task<IActionResult> DeleteFile(int? id, int Requestid)
        {
            if (await _admindashboardactions.DeleteDocumentByRequest(id.ToString()))
            {
                _notyf.Success("File Deleted Successfully");
            }
            else
            {
                _notyf.Error("File Not Deleted");
            }
            return RedirectToAction("ViewUpload", "Home", new { id = Requestid });
        }
        #endregion

        #region SendOrder
        public async Task<IActionResult> Order(int id)
        {
            List<HealthProfessionalTypes> cs = _dropdown.HealthProfessionalType();
            ViewBag.ProfessionType = cs;
            ViewDataViewOrders data = new()
            {
                RequestId = id
            };
            return View("../Home/SendOrder", data);
        }
        public Task<IActionResult> ProfessionalByType(int HealthprofessionalID)
        {
            var v = _dropdown.ProfessionalByType(HealthprofessionalID);
            return Task.FromResult<IActionResult>(Json(v));
        }

        public Task<IActionResult> SelectProfessionalByID(int VendorID)
        {
            var v = _admindashboardactions.SelectProfessionalByID(VendorID);
            return Task.FromResult<IActionResult>(Json(v));
        }
        public IActionResult SendOrder(ViewDataViewOrders sm)
        {
            if (ModelState.IsValid)
            {
                bool data = _admindashboardactions.SendOrder(sm);
                if (data)
                {
                    _notyf.Success("Order Created  successfully...");
                    _notyf.Information("Mail is sent to Vendor successfully...");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _notyf.Error("Order Not Created...");
                    return View("../Home/SendOrder", sm);
                }
            }
            else
            {
                return View("../Home/SendOrder", sm);
            }
        }
        #endregion

        #region SendAgreement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAgreementmail(int requestid, string PatientName)
        {
            if (_admindashboardactions.SendAgreement(requestid, PatientName))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region CloseCase
        public async Task<IActionResult> CloseCase(int RequestID)
        {
            ViewCloseCaseModel vc = _admindashboardactions.CloseCaseData(RequestID);
            return View("../Home/CloseCase", vc);
        }
        public IActionResult CloseCaseUnpaid(int id)
        {
            bool sm = _admindashboardactions.CloseCase(id);
            if (sm)
            {
                _notyf.Success("Case Closed...");
                _notyf.Information("You can see Closed case in unpaid State...");

            }
            else
            {
                _notyf.Error("there is some error in CloseCase...");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region EditCloseCase
        public IActionResult EditForCloseCase(ViewCloseCaseModel sm)
        {
            bool result = _admindashboardactions.EditForCloseCase(sm);

            if (result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("CloseCase", new { sm.RequestId });
            }
            else
            {
                _notyf.Error("Case Not Edited...");
                return RedirectToAction("CloseCase", new { sm.RequestId });
            }

        }
        #endregion

        #region Encounter
        public IActionResult Encounter(int? RId)
        {
            ViewEncounter ve = _admindashboardactions.EncounterIndex(RId);
            return View(ve);
        }
        #endregion

        #region EncounterSave
        [HttpPost]
        public IActionResult Encounter(int? RequestId, ViewEncounter ve)
        {
            ViewEncounter ven = _admindashboardactions.EncounterSave(RequestId, ve);
            _notyf.Success("Updated Successfully.......");
            return View("../Home/Encounter", ven);
        }
        #endregion

        public IActionResult Export(string status)
        {
            var requestData = _irequestRepository.Export(status);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PatientPhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].DateOfBirth;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestId;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        #region AuthError
        public IActionResult AuthError()
        {
            return View("../Home/AuthError");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}