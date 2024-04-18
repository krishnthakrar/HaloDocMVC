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
using Rotativa;
using ViewAsPdf = Rotativa.AspNetCore.ViewAsPdf;
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
        #region Index
        [ProviderAccess("Admin,Provider")]
        [Route("DashBoard")]
        public IActionResult Index()
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewBag.CaseReason = _dropdown.CaseReason();
            PaginatedViewModel pvm = _irequestRepository.IndexData(-1);
            if (CredentialValue.role() == "Provider")
            {
                pvm = _irequestRepository.IndexData(Convert.ToInt32(CredentialValue.UserId()));
            }
            return View(pvm);
        }
        #endregion

        #region SearchResult
        public IActionResult _SearchResult(string Status, PaginatedViewModel data)
        {
            if (Status == null)
            {
                Status = CredentialValue.CurrentStatus();
            }
            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);
            PaginatedViewModel contacts = _irequestRepository.GetRequests(Status, data);
            if (CredentialValue.role() == "Provider")
            {
                contacts = _irequestRepository.GetRequests(Status, data, Convert.ToInt32(CredentialValue.UserId()));
            }
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
        public IActionResult ViewCase(int RId, int RTId, int Status)
        {
            ViewBag.AllRegion = _dropdown.AllRegion();
            ViewDataViewCase vdvc = _admindashboardactions.NewRequestData(RId, RTId, Status);
            return View(vdvc);
        }
        #endregion

        #region EditViewCase
        [HttpPost]
        public IActionResult ViewCase(ViewDataViewCase vdvc, int RId, int RTId, int Status)
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
                    return RedirectToAction("ViewNotes", "Home", new { RId = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Not Updated");
                    return RedirectToAction("ViewNotes", "Home", new { RId = RequestID });
                }
            }
            else
            {
                _notyf.Information("Notes can't be empty");
                TempData["Errormassage"] = "Notes can't be empty";
                return RedirectToAction("ViewNotes", "Home", new { RId = RequestID });
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
        public IActionResult ViewUpload(int? id, ViewDataViewDocuments viewDocument)
        {
            if (id == null)
            {
                id = viewDocument.RequestId;
            }
            ViewDataViewDocuments v = _admindashboardactions.GetDocumentByRequest(id, viewDocument);
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

        #region SendFilEmail
        public async Task<IActionResult> SendFileEmail(string mailids, int Requestid, string email)
        {
            if (await _admindashboardactions.SendFileEmail(mailids, Requestid, email))
            {

                _notyf.Success("Mail Send successfully");
            }
            else
            {
                _notyf.Error("Mail is not send successfully");
            }
            return RedirectToAction("ViewUpload", "Home", new { id = Requestid });
        }
        #endregion

        #region SendOrder
        public IActionResult Order(int id)
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
        public IActionResult SendAgreementmail(int requestid, string PatientName)
        {
            if (_admindashboardactions.SendAgreement(requestid, PatientName))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region CloseCase
        public IActionResult CloseCase(int RequestID)
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

        #region AcceptCase
        [HttpPost]
        public IActionResult AcceptCase(int RequestId, string Note)
        {
            if (_admindashboardactions.AcceptPhysician(RequestId, Note, Convert.ToInt32(CredentialValue.UserId())))
            {
                _notyf.Success("Case Accepted...");
            }
            else
            {
                _notyf.Success("Case Not Accepted...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        #endregion

        #region TransToAdmin
        public IActionResult TransToAdmin(int RequestId, string Note)
        {
            if (_admindashboardactions.TransToAdmin(RequestId, Note, Convert.ToInt32(CredentialValue.UserId())))
            {
                _notyf.Success("Request Successfully transfered to admin...");
            }
            return Redirect("~/Physician/DashBoard");
        }
        #endregion

        #region RequestAdmin
        public IActionResult RequestAdmin(string Note)
        {
            bool Contact = _admindashboardactions.RequestAdmin(Convert.ToInt32(CredentialValue.UserId()), Note);
            if (Contact)
            {
                _notyf.Success("Mail Send Succesfully Successfully");
            }
            else
            {
                _notyf.Error("Mail Not Send Succesfully");
            }
            return RedirectToAction("EditProvider", "Access", new { id = Convert.ToInt32(CredentialValue.UserId()) });
        }
        #endregion

        #region HouseCall
        [HttpPost]
        public IActionResult HouseCall(int RequestId)
        {
            bool Status = _admindashboardactions.HouseCall(RequestId);
            if (Status)
            {
                _notyf.Success("Status changed for HouseCall.....");
            }
            else
            {
                _notyf.Error("Status not changed.......");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Consult
        [HttpPost]
        public IActionResult Consult(int RequestId)
        {
            bool Status = _admindashboardactions.Consult(RequestId);
            if (Status)
            {
                _notyf.Success("Status changed for Consult.....");
            }
            else
            {
                _notyf.Error("Status not changed.....");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region HouseCallSubmit
        public IActionResult HouseCallSubmit(int id)
        {
            bool Status = _admindashboardactions.HouseCallSubmit(id);
            if (Status)
            {
                _notyf.Success("Status changed Successfully.....");
            }
            else
            {
                _notyf.Error("Status not changed.....");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Finalize
        public IActionResult Finalize(int id)
        {
            bool Finalize = _admindashboardactions.Finalize(id);
            if (Finalize)
            {
                _notyf.Success("Request Finalized.....");
            }
            else
            {
                _notyf.Error("Request not Finalized.....");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region generatePDF
        public IActionResult generatePDF(int RequestId)
        {
            var FormDetails = _admindashboardactions.EncounterIndex(RequestId);
            return new ViewAsPdf("../Home/EncounterPDF", FormDetails);
        }
        #endregion

        #region ConcludeCareIndex
        public IActionResult ConcludeCare(int? id, ViewDataViewDocuments viewDocument)
        {
            if (id == null)
            {
                id = viewDocument.RequestId;
            }
            ViewDataViewDocuments v =  _admindashboardactions.GetDocumentByRequest(id, viewDocument);
            return View("../Home/ConcludeCare", v);
        }
        #endregion

        #region UploadDocProvider
        public IActionResult UploadDocProvider(int Requestid, IFormFile file)
        {
            if (_admindashboardactions.SaveDoc(Requestid, file))
            {
                _notyf.Success("File Uploaded Successfully");
            }
            else
            {
                _notyf.Error("File Not Uploaded");
            }
            return RedirectToAction("ConcludeCare", "Home", new { id = Requestid });
        }
        #endregion

        #region ConcludecarePost
        public IActionResult ConcludeCarePost(int RequestId, string Notes)
        {
            if (_admindashboardactions.ConcludeCarePost(RequestId, Notes))
            {
                _notyf.Success("Case concluded...");
            }
            else
            {
                _notyf.Error("Error...");
            }
            return Redirect("~/DashBoard");
        }
        #endregion

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