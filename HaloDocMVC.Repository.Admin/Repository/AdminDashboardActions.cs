using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static HaloDocMVC.Entity.Models.Constant;
using static HaloDocMVC.Entity.Models.ViewDataViewDocuments;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class AdminDashboardActions : IAdminDashboardActions
    {
        private readonly HaloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public AdminDashboardActions(HaloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }

        #region ViewCase
        public ViewDataViewCase NewRequestData(int? RId, int? RTId, int? Status)
        {
            ViewDataViewCase caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewDataViewCase()
                                        {
                                            UserId = req.Request.UserId,
                                            RequestId = (int)RId,
                                            RequestTypeId = (int)RTId,
                                            Status = (int)Status,
                                            ConfNo = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                                            Notes = req.Notes,
                                            FirstName = req.FirstName,
                                            LastName = req.LastName,
                                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                                            Mobile = req.PhoneNumber,
                                            Email = req.Email,
                                            Address = req.Address
                                        }).FirstOrDefault();

            return caseList;
        }
        #endregion

        #region EditViewCase
        public ViewDataViewCase Edit(ViewDataViewCase vdvc, int? RId, int? RTId, int? Status)
        {
            try
            {
                RequestClient RC = _context.RequestClients.FirstOrDefault(E => E.RequestId == vdvc.RequestId);
                RC.PhoneNumber = vdvc.Mobile;
                RC.Email = vdvc.Email;
                _context.Update(RC);
                _context.SaveChanges();

                ViewDataViewCase caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewDataViewCase()
                                        {
                                            UserId = req.Request.UserId,
                                            RequestId = (int)RId,
                                            RequestTypeId = (int)RTId,
                                            Status = (int)Status,
                                            ConfNo = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                                            Notes = req.Notes,
                                            FirstName = req.FirstName,
                                            LastName = req.LastName,
                                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                                            Mobile = req.PhoneNumber,
                                            Email = req.Email,
                                            Address = req.Address
                                        }).FirstOrDefault();

                return caseList;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(vdvc.RequestId))
                {
                    throw;
                }
                else
                {
                    throw;
                }
            }
        }
        private bool RequestExists(object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region AssignProvider
        public bool AssignProvider(int RequestId, int ProviderId, string notes)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            RequestStatusLog rsl = new RequestStatusLog
            {
                RequestId = RequestId,
                PhysicianId = ProviderId,
                Notes = notes,
                CreatedDate = DateTime.Now,
                Status = 2
            };
            
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region CancelCase
        public bool CancelCase(int RequestID, string Note, string CaseTag)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.CaseTag = CaseTag;
                    requestData.Status = 3;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Notes = Note,
                        Status = 3,
                        CreatedDate = DateTime.Now
                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region BlockCase
        public bool BlockCase(int RequestID, string Note)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    BlockRequest br = new BlockRequest
                    {
                        RequestId = RequestID,
                        PhoneNumber = requestData.PhoneNumber,
                        Email = requestData.Email,
                        Reason = Note,
                        CreatedDate = DateTime.Now
                    };
                    _context.BlockRequests.Add(br);
                    _context.SaveChanges();

                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Notes = Note,
                        CreatedDate = DateTime.Now,
                        Status = 11
                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region TransferProvider
        public async Task<bool> TransferProvider(int RequestId, int ProviderId, string notes)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
            
            
            RequestStatusLog rsl = new RequestStatusLog
            {
                RequestId = RequestId,
                PhysicianId = request.PhysicianId,
                Notes = notes,
                CreatedDate = DateTime.Now,
                TransToPhysicianId = ProviderId,
                Status = 2
            };

            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region ClearCase
        public bool ClearCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 10;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    
                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Status = 10,
                        CreatedDate = DateTime.Now
                    };
                    
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else 
                { 
                    return false; 
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ViewNotes
        public ViewDataViewNotes GetNotesByID(int id)
        {
            var req = _context.Requests.FirstOrDefault(W => W.RequestId == id);
            var symptoms = _context.RequestClients.FirstOrDefault(W => W.RequestId == id);
            var transferlog = (from rs in _context.RequestStatusLogs
                               join py in _context.Physicians on rs.PhysicianId equals py.PhysicianId into pyGroup
                               from py in pyGroup.DefaultIfEmpty()
                               join p in _context.Physicians on rs.TransToPhysicianId equals p.PhysicianId into pGroup
                               from p in pGroup.DefaultIfEmpty()
                               join a in _context.Admins on rs.AdminId equals a.AdminId into aGroup
                               from a in aGroup.DefaultIfEmpty()
                               where rs.RequestId == id && rs.Status == 2
                               select new TransferNotes
                               {
                                   TransPhysician = p.FirstName,
                                   Admin = a.FirstName,
                                   Physician = py.FirstName,
                                   RequestId = rs.RequestId,
                                   Notes = rs.Notes,
                                   Status = rs.Status,
                                   PhysicianId = rs.PhysicianId,
                                   CreatedDate = rs.CreatedDate,
                                   RequestStatusLogId = rs.RequestStatusLogId,
                                   TransToAdmin = rs.TransToAdmin,
                                   TransToPhysicianId = rs.TransToPhysicianId
                               }).ToList();
            var cancelbyprovider = _context.RequestStatusLogs.Where(E => E.RequestId == id && (E.TransToAdmin != null));
            var cancel = _context.RequestStatusLogs.Where(E => E.RequestId == id && (E.Status == 7 || E.Status == 3));
            var model = _context.RequestNotes.FirstOrDefault(E => E.RequestId == id);
            ViewDataViewNotes vdvn = new();
            vdvn.RequestId = id;
            /*vdvn.PatientNotes = symptoms.Notes;*/
            if (model == null)
            {
                vdvn.PhysicianNotes = "-";
                vdvn.AdminNotes = "-";
            }
            else
            {
                vdvn.Status = (short)req.Status;
                vdvn.RequestNotesId = model.RequestNotesId;
                vdvn.PhysicianNotes = model.PhysicianNotes ?? "-";
                vdvn.AdminNotes = model.AdminNotes ?? "-";
            }

            List<TransferNotes> transfer = new();
            foreach (var item in transferlog)
            {
                transfer.Add(new TransferNotes
                {
                    TransPhysician = item.TransPhysician,
                    Admin = item.Admin,
                    Physician = item.Physician,
                    RequestId = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.PhysicianId,
                    CreatedDate = item.CreatedDate,
                    RequestStatusLogId = item.RequestStatusLogId,
                    TransToAdmin = item.TransToAdmin,
                    TransToPhysicianId = item.TransToPhysicianId
                });
            }
            vdvn.TransferNotes = transfer;
            List<TransferNotes> cancelbyphysician = new();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotes
                {
                    RequestId = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.PhysicianId,
                    CreatedDate = item.CreatedDate,
                    RequestStatusLogId = item.RequestStatusLogId,
                    TransToAdmin = item.TransToAdmin,
                    TransToPhysicianId = item.TransToPhysicianId
                });
            }
            vdvn.CancelByPhysician = cancelbyphysician;

            List<TransferNotes> cancelrq = new();
            foreach (var item in cancel)
            {
                cancelrq.Add(new TransferNotes
                {
                    RequestId = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.PhysicianId,
                    CreatedDate = item.CreatedDate,
                    RequestStatusLogId = item.RequestStatusLogId,
                    TransToAdmin = item.TransToAdmin,
                    TransToPhysicianId = item.TransToPhysicianId
                });
            }
            vdvn.Cancel = cancelrq;

            return vdvn;
        }
        #endregion

        #region EditViewNotes
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID)
        {
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestID);
                if (notes != null)
                {
                    if (physiciannotes != null)
                    {
                        if (notes != null)
                        {
                            notes.PhysicianNotes = physiciannotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (adminnotes != null)
                    {
                        if (notes != null)
                        {
                            notes.AdminNotes = adminnotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RequestNote rn = new RequestNote
                    {
                        RequestId = RequestID,
                        AdminNotes = adminnotes,
                        PhysicianNotes = physiciannotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "65e196bf-b39d-48e8-a3da-ebd3b699dede"
                    };
                    _context.RequestNotes.Add(rn);
                    _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region GetDocumentByRequest
        public async Task<ViewDataViewDocuments> GetDocumentByRequest(int? id, ViewDataViewDocuments viewDocument)
        {
            var req = _context.RequestClients.FirstOrDefault(r => r.RequestId == id);
            var result = (from requestWiseFile in _context.RequestWiseFiles
                          join request in _context.Requests on requestWiseFile.RequestId equals request.RequestId
                          join physician in _context.Physicians on request.PhysicianId equals physician.PhysicianId into physicianGroup
                          from phys in physicianGroup.DefaultIfEmpty()
                          join admin in _context.Admins on requestWiseFile.AdminId equals admin.AdminId into adminGroup
                          from adm in adminGroup.DefaultIfEmpty()
                          where request.RequestId == id && requestWiseFile.IsDeleted == new BitArray(1)
                          select new Documents
                         {
                             Uploader = requestWiseFile.PhysicianId != null ? phys.FirstName : (requestWiseFile.AdminId != null ? adm.FirstName : request.FirstName),
                             IsDeleted = requestWiseFile.IsDeleted.ToString(),
                             RequestWiseFilesId = requestWiseFile.RequestWiseFileId,
                             Status = requestWiseFile.DocType,
                             CreatedDate = requestWiseFile.CreatedDate,
                             FileName = requestWiseFile.FileName
                         }).ToList();
            if (viewDocument.IsAscending == true)
            {
                result = viewDocument.SortedColumn switch
                {
                    "CreatedDate" => result.OrderBy(x => x.CreatedDate).ToList(),
                    "Uploader" => result.OrderBy(x => x.Uploader).ToList(),
                    "FileName" => result.OrderBy(x => x.FileName).ToList(),
                    _ => result.OrderBy(x => x.CreatedDate).ToList()
                };
            }
            else
            {
                result = viewDocument.SortedColumn switch
                {
                    "CreatedDate" => result.OrderByDescending(x => x.CreatedDate).ToList(),
                    "Uploader" => result.OrderByDescending(x => x.Uploader).ToList(),
                    "FileName" => result.OrderByDescending(x => x.FileName).ToList(),
                    _ => result.OrderByDescending(x => x.CreatedDate).ToList()
                };
            }
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)viewDocument.PageSize);
            List<Documents> list1 = result.Skip((viewDocument.CurrentPage - 1) * viewDocument.PageSize).Take(viewDocument.PageSize).ToList();
            ViewDataViewDocuments vd = new()
            {
                DocumentsList = list1,
                CurrentPage = viewDocument.CurrentPage,
                TotalPages = totalPages,
                PageSize = viewDocument.PageSize,
                IsAscending = viewDocument.IsAscending,
                SortedColumn = viewDocument.SortedColumn,
                FirstName = req.FirstName,
                LastName = req.LastName,
                ConfirmationNumber = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                RequestId = req.RequestId
            };
            return vd;
        }
        #endregion

        #region SaveDocument
        public bool SaveDoc(int Requestid, IFormFile file)
        {
            string UploadDoc = SaveFile.UploadDoc(file, Requestid);
            RequestWiseFile rwf = new RequestWiseFile
            {
                RequestId = Requestid,
                FileName = UploadDoc,
                CreatedDate = DateTime.Now,
                IsDeleted = new BitArray(1),
                AdminId = 1
            };
            _context.RequestWiseFiles.Add(rwf);
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region DeleteDocumentByRequest
        public async Task<bool> DeleteDocumentByRequest(string ids)
        {
            List<int> deletelist = ids.Split(',').Select(int.Parse).ToList();
            foreach (int item in deletelist)
            {
                if (item > 0)
                {
                    var data = await _context.RequestWiseFiles.Where(e => e.RequestWiseFileId == item).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.IsDeleted[0] = true;
                        _context.RequestWiseFiles.Update(data);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region SendFileEmail
        public async Task<bool> SendFileEmail(string ids, int Requestid, string email)
        {
            var v = await GetRequestDetails(Requestid);
            List<int> priceList = ids.Split(',').Select(int.Parse).ToList();
            List<string> files = new();
            foreach (int price in priceList)
            {
                if (price > 0)
                {
                    var data = await _context.RequestWiseFiles.Where(e => e.RequestWiseFileId == price).FirstOrDefaultAsync();
                    files.Add(Directory.GetCurrentDirectory() + "\\wwwroot\\Upload" + data.FileName.Replace("Upload/", "").Replace("/", "\\"));
                }
            }

            if (await _emailConfig.SendMailAsync(email, "All Document Of Your Request " + v.PatientName, "Heyy " + v.PatientName + " Kindly Check your Attachments", files))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region GetRequestDetails
        public async Task<ViewActions> GetRequestDetails(int? id)
        {

            return await (from req in _context.Requests
                          join reqClient in _context.RequestClients
                          on req.RequestId equals reqClient.RequestId into reqClientGroup
                          from rc in reqClientGroup.DefaultIfEmpty()
                          join phys in _context.Physicians
                          on req.PhysicianId equals phys.PhysicianId into physGroup
                          from p in physGroup.DefaultIfEmpty()
                          where req.RequestId == id
                          select new ViewActions
                          {
                              PhoneNumber = rc.PhoneNumber,
                              ProviderId = p.PhysicianId,
                              PatientName = rc.FirstName + " " + rc.LastName,
                              RequestID = req.RequestId,
                              Email = rc.Email

                          }).FirstAsync();
        }
        #endregion

        #region SendOrderIndex
        public HealthProfessional SelectProfessionalByID(int VendorID)
        {
            return _context.HealthProfessionals.FirstOrDefault(e => e.VendorId == VendorID);
        }
        #endregion

        #region SendOrder
        public bool SendOrder(ViewDataViewOrders data)
        {
            try
            {
                OrderDetail od = new()
                {
                    RequestId = data.RequestId,
                    VendorId = data.VendorId,
                    FaxNumber = data.FaxNumber,
                    Email = data.Email,
                    BusinessContact = data.BusinessContact,
                    Prescription = data.Prescription,
                    NoOfRefill = data.NoOfRefill,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "65e196bf-b39d-48e8-a3da-ebd3b699dede"
                };
                _context.OrderDetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.RequestId == data.RequestId);
                _emailConfig.SendMail(data.Email, "Order Details", "<p>Prescription:" + data.Prescription + "<br> No of Refills: " + data.NoOfRefill + "</p>");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region SendAgreement
        public Boolean SendAgreement(int requestid, string PatientName)
        {
            var res = _context.RequestClients.FirstOrDefault(e => e.RequestId == requestid);
            var agreementUrl = "https://localhost:44348/Agreement?RequestID=" + requestid + "&PatientName=" +  PatientName;
            _emailConfig.SendMail(res.Email, "Agreement for your request", $"<a href='{agreementUrl}'>Link Containing Agreement Page</a>");
            return true;
        }
        #endregion

        #region SendAgreement_accept
        public Boolean SendAgreement_accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new();
                rsl.RequestId = RequestID;

                rsl.Status = 4;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        #region SendAgreement_Reject
        public Boolean SendAgreement_Reject(int RequestID, string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new();
                rsl.RequestId = RequestID;

                rsl.Status = 7;
                rsl.Notes = Notes;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        #region CloseCaseData
        public ViewCloseCaseModel CloseCaseData(int RequestID)
        {
            ViewCloseCaseModel alldata = new();

            var result = from requestWiseFile in _context.RequestWiseFiles
                         join request in _context.Requests on requestWiseFile.RequestId equals request.RequestId
                         join physician in _context.Physicians on request.PhysicianId equals physician.PhysicianId into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.AdminId equals admin.AdminId into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.RequestId == RequestID
                         select new
                         {

                             Uploader = requestWiseFile.PhysicianId != null ? phys.FirstName :
                             (requestWiseFile.AdminId != null ? adm.FirstName : request.FirstName),
                             requestWiseFile.FileName,
                             requestWiseFile.CreatedDate,
                             requestWiseFile.RequestWiseFileId

                         };
            List<Documents> doc = new();
            foreach (var item in result)
            {
                doc.Add(new Documents
                {
                    CreatedDate = item.CreatedDate,
                    FileName = item.FileName,
                    Uploader = item.Uploader,
                    RequestWiseFilesId = item.RequestWiseFileId
                });

            }
            alldata.DocumentsList = doc;
            Entity.DataModels.Request req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestID);

            alldata.FirstName = req.FirstName;
            alldata.RequestId = req.RequestId;
            alldata.ConfirmationNumber = req.ConfirmationNumber;
            alldata.LastName = req.LastName;

            var reqcl = _context.RequestClients.FirstOrDefault(e => e.RequestId == RequestID);

            alldata.RC_Email = reqcl.Email;
            alldata.RC_DOB = new DateTime((int)reqcl.IntYear, Convert.ToInt32(reqcl.StrMonth.Trim()), (int)reqcl.IntDate);
            alldata.RC_FirstName = reqcl.FirstName;
            alldata.RC_LastName = reqcl.LastName;
            alldata.RC_PhoneNumber = reqcl.PhoneNumber;
            return alldata;
        }
        #endregion

        #region EditCloseCase
        public bool EditForCloseCase(ViewCloseCaseModel model)
        {
            try
            {
                RequestClient client = _context.RequestClients.FirstOrDefault(E => E.RequestId == model.RequestId);
                if (client != null)
                {
                    client.PhoneNumber = model.RC_PhoneNumber;
                    client.Email = model.RC_Email;
                    _context.RequestClients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region CloseCase
        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 9;
                    requestData.ModifiedDate = DateTime.Now;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    
                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Status = 9,
                        CreatedDate = DateTime.Now
                    };

                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region EncounterIndex
        public ViewEncounter EncounterIndex(int? RId)
        {
            if (RId == null) return null;
            var encounter = (from rc in _context.RequestClients
                             join en in _context.Encounters on rc.RequestId equals en.RequestId into renGroup
                             from subEn in renGroup.DefaultIfEmpty()
                             where rc.RequestId == RId
                             select new ViewEncounter
                             {
                                 RequestId = rc.RequestId,
                                 FirstName = rc.FirstName,
                                 LastName = rc.LastName,
                                 Location = rc.Address,
                                 DOB = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                 DOS = (DateTime)subEn.DateOfService,
                                 Mobile = rc.PhoneNumber,
                                 Email = rc.Email,
                                 Injury = subEn.Injury,
                                 History = subEn.MedicalHistory,
                                 Medications = subEn.Medications,
                                 Allergies = subEn.Allergies,
                                 Temp = subEn.Temp,
                                 HR = subEn.Hr,
                                 RR = subEn.Rr,
                                 Bp = subEn.BloodPressure,
                                 O2 = subEn.O2,
                                 Pain = subEn.Pain,
                                 Heent = subEn.Heent,
                                 CV = subEn.Cv,
                                 Chest = subEn.Chest,
                                 ABD = subEn.Abd,
                                 Extr = subEn.Extr,
                                 Skin = subEn.Skin,
                                 Neuro = subEn.Neuro,
                                 Other = subEn.Other,
                                 Diagnosis = subEn.Diagnosis,
                                 Treatment = subEn.Treatment,
                                 MDispensed = subEn.MedicationsDispensed,
                                 Procedures = subEn.Procedures,
                                 Followup = subEn.Followup
                             }).FirstOrDefault();

            return encounter;
        }

        #endregion

        #region EncounterSave
        public ViewEncounter EncounterSave(int? RId, ViewEncounter ve)
        {
            var RC = _context.RequestClients.FirstOrDefault(rc => rc.RequestId == RId);
            if (RC == null) return null;
            RC.FirstName = ve.FirstName;
            RC.LastName = ve.LastName;
            RC.Address = ve.Location;
            RC.StrMonth = ve.DOB.Month.ToString();
            RC.IntDate = ve.DOB.Day;
            RC.IntYear = ve.DOB.Year;
            RC.PhoneNumber = ve.Mobile;
            RC.Email = ve.Email;
            _context.Update(RC);
            var E = _context.Encounters.FirstOrDefault(e => e.RequestId == RId);
            if (E == null)
            {
                E = new Encounter { RequestId = (int)RId };
                _context.Encounters.Add(E);
            }
            E.DateOfService = ve.DOS;
            E.MedicalHistory = ve.History;
            E.Injury = ve.Injury;
            E.Medications = ve.Medications;
            E.Allergies = ve.Allergies;
            E.Temp = ve.Temp;
            E.Hr = ve.HR;
            E.Rr = ve.RR;
            E.BloodPressure = ve.Bp;
            E.O2 = ve.O2;
            E.Pain = ve.Pain;
            E.Heent = ve.Heent;
            E.Cv = ve.CV;
            E.Chest = ve.Chest;
            E.Abd = ve.ABD;
            E.Extr = ve.Extr;
            E.Skin = ve.Skin;
            E.Neuro = ve.Neuro;
            E.Other = ve.Other;
            E.Diagnosis = ve.Diagnosis;
            E.Treatment = ve.Treatment;
            E.MedicationsDispensed = ve.MDispensed;
            E.Procedures = ve.Procedures;
            E.Followup = ve.Followup;
            _context.SaveChanges();
            return ve;
        }
        #endregion
    }
}
