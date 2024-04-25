using DocumentFormat.OpenXml.Spreadsheet;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaloDocMVC.Entity.Models.Constant;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Records : IRecords
    {
        private readonly HaloDocContext _context;
        public Records(HaloDocContext context)
        {
            _context = context;
        }

        #region GetFilteredSearchRecords
        public RecordsModel GetFilteredSearchRecords(RecordsModel rm)
        {
            List<SearchRecords> allData = (from req in _context.Requests
                                           join reqClient in _context.RequestClients
                                           on req.RequestId equals reqClient.RequestId into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _context.Physicians
                                           on req.PhysicianId equals phys.PhysicianId into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join reg in _context.Regions
                                           on rc.RegionId equals reg.RegionId into RegGroup
                                           from rg in RegGroup.DefaultIfEmpty()
                                           join nts in _context.RequestNotes
                                           on req.RequestId equals nts.RequestId into ntsgrp
                                           from nt in ntsgrp.DefaultIfEmpty()
                                           where (req.IsDeleted == new BitArray(1) && rm.Status == null || req.Status == rm.Status) &&
                                                 (rm.RequestType == null || req.RequestTypeId == rm.RequestType) &&
                                                 (!rm.StartDate.HasValue || req.CreatedDate.Date >= rm.StartDate.Value.Date) &&
                                                 (!rm.EndDate.HasValue || req.CreatedDate.Date <= rm.EndDate.Value.Date) &&
                                                 (rm.PatientName.IsNullOrEmpty() || (rc.FirstName + " " + rc.LastName).ToLower().Contains(rm.PatientName.ToLower())) &&
                                                 (rm.PhysicianName.IsNullOrEmpty() || (p.FirstName + " " + p.LastName).ToLower().Contains(rm.PhysicianName.ToLower())) &&
                                                 (rm.Email.IsNullOrEmpty() || rc.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                                 (rm.PhoneNumber.IsNullOrEmpty() || rc.PhoneNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                           orderby req.CreatedDate
                                           select new SearchRecords
                                           {
                                               ModifiedDate = req.ModifiedDate,
                                               PatientName = rc.FirstName + " " + rc.LastName,
                                               RequestId = req.RequestId,
                                               DateOfService = req.CreatedDate,
                                               PhoneNumber = rc.PhoneNumber ?? "",
                                               Email = rc.Email ?? "",
                                               Address = rc.Address ?? "",
                                               Zip = rc.ZipCode ?? "",
                                               RequestTypeId = req.RequestTypeId,
                                               Status = (short)req.Status,
                                               PhysicianName = p.FirstName + " " + p.LastName ?? "",
                                               AdminNote = nt != null ? nt.AdminNotes ?? "" : "",
                                               PhysicianNote = nt != null ? nt.PhysicianNotes ?? "" : "",
                                               PatientNote = rc.Notes ?? ""
                                           }).ToList();
            if (rm.IsAscending == true)
            {
                allData = rm.SortedColumn switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "DateOfService" => allData.OrderBy(x => x.DateOfService).ToList(),
                    "Email" => allData.OrderBy(x => x.Email).ToList(),
                    "Status" => allData.OrderBy(x => x.Status).ToList(),
                    _ => allData.OrderBy(x => x.PatientName).ToList()
                };
            }
            else
            {
                allData = rm.SortedColumn switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "DateOfService" => allData.OrderByDescending(x => x.DateOfService).ToList(),
                    "Email" => allData.OrderByDescending(x => x.Email).ToList(),
                    "Status" => allData.OrderByDescending(x => x.Status).ToList(),
                    _ => allData.OrderByDescending(x => x.PatientName).ToList()
                };
            }
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SearchRecords> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                SearchRecords = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
                IsAscending = rm.IsAscending,
                SortedColumn = rm.SortedColumn,
                RequestType = rm.RequestType,
                Status = rm.Status,
                PhysicianName = rm.PhysicianName,
                Email = rm.Email,
                PhoneNumber = rm.PhoneNumber,
                PatientName = rm.PatientName,
                StartDate = rm.StartDate,
                EndDate = rm.EndDate,

            };

            for (int i = 0; i < records.SearchRecords.Count; i++)
            {
                if (records.SearchRecords[i].Status == 9)
                {
                    records.SearchRecords[i].CloseCaseDate = records.SearchRecords[i].ModifiedDate;
                }
                else
                {
                    records.SearchRecords[i].CloseCaseDate = null;
                }
                if (records.SearchRecords[i].Status == 3 && records.SearchRecords[i].PhysicianName != null)
                {
                    var data = _context.RequestStatusLogs.FirstOrDefault(x => (x.Status == 3) && (x.RequestId == records.SearchRecords[i].RequestId));
                    records.SearchRecords[i].CancelByProviderNote = data.Notes;
                }

            }
            return records;
        }
        #endregion

        #region DeleteRequest
        public bool DeleteRequest(int? RequestId)
        {
            try
            {
                var data = _context.Requests.FirstOrDefault(v => v.RequestId == RequestId);
                data.IsDeleted = new BitArray(1);
                data.IsDeleted[0] = true;
                data.ModifiedDate = DateTime.Now;
                _context.Requests.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion DeleteRequest

        #region BlockHistory
        public RecordsModel BlockHistory(RecordsModel rm)
        {
            List<BlockRequests> data = (from req in _context.BlockRequests
                                        where req.IsActive == new BitArray(1) &&
                                              (!rm.StartDate.HasValue || req.CreatedDate.Value.Date == rm.StartDate.Value.Date) &&
                                              (rm.PatientName.IsNullOrEmpty() || _context.Requests.FirstOrDefault(e => e.RequestId == Convert.ToInt32(req.RequestId)).FirstName.ToLower().Contains(rm.PatientName.ToLower())) &&
                                              (rm.Email.IsNullOrEmpty() || req.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                              (rm.PhoneNumber.IsNullOrEmpty() || req.PhoneNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                        select new BlockRequests
                                        {
                                            PatientName = _context.RequestClients.FirstOrDefault(e => e.RequestId == Convert.ToInt32(req.RequestId)).FirstName,
                                            Email = req.Email,
                                            CreatedDate = (DateTime)req.CreatedDate,
                                            IsActive = req.IsActive,
                                            RequestId = Convert.ToInt32(req.RequestId),
                                            PhoneNumber = req.PhoneNumber,
                                            Reason = req.Reason
                                        }).ToList();
            if (rm.IsAscending == true)
            {
                data = rm.SortedColumn switch
                {
                    "PatientName" => data.OrderBy(x => x.PatientName).ToList(),
                    "CreatedDate" => data.OrderBy(x => x.CreatedDate).ToList(),
                    "Email" => data.OrderBy(x => x.Email).ToList(),
                    _ => data.OrderBy(x => x.PatientName).ToList()
                };
            }
            else
            {
                data = rm.SortedColumn switch
                {
                    "PatientName" => data.OrderByDescending(x => x.PatientName).ToList(),
                    "CreatedDate" => data.OrderByDescending(x => x.CreatedDate).ToList(),
                    "Email" => data.OrderByDescending(x => x.Email).ToList(),
                    _ => data.OrderByDescending(x => x.PatientName).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<BlockRequests> list = data.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel model = new()
            {
                BlockRequests = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
                IsAscending = rm.IsAscending,
                SortedColumnBlock = rm.SortedColumnBlock,
                StartDate = rm.StartDate,
                PatientName = rm.PatientName,
                Email = rm.Email,
                PhoneNumber = rm.PhoneNumber
            };

            return model;
        }
        #endregion

        #region Unblock
        public bool Unblock(int RequestId, string id)
        {
            try
            {
                BlockRequest block = _context.BlockRequests.FirstOrDefault(e => e.RequestId == RequestId);
                block.IsActive = new BitArray(1);
                block.IsActive[0] = true;
                _context.BlockRequests.Update(block);
                _context.SaveChanges();

                Request request = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
                request.Status = 1;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                var admindata = _context.Admins.FirstOrDefault(e => e.AspNetUserId == id);
                RequestStatusLog rs = new()
                {
                    Status = 1,
                    RequestId = RequestId,
                    AdminId = admindata.AdminId,
                    CreatedDate = DateTime.Now
                };
                _context.RequestStatusLogs.Add(rs);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region GetFilteredPatientHistory
        public RecordsModel GetFilteredPatientHistory(RecordsModel rm)
        {
            List<User> allData = (from user in _context.Users
                                  where (string.IsNullOrEmpty(rm.Email) || user.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                  (string.IsNullOrEmpty(rm.PhoneNumber) || user.Mobile.ToLower().Contains(rm.PhoneNumber.ToLower())) &&
                                  (string.IsNullOrEmpty(rm.FirstName) || user.FirstName.ToLower().Contains(rm.FirstName.ToLower())) &&
                                  (string.IsNullOrEmpty(rm.LastName) || user.LastName.ToLower().Contains(rm.LastName.ToLower()))
                                  select new User
                                  {
                                      UserId = user.UserId,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Email = user.Email,
                                      Mobile = user.Mobile,
                                      Street = user.Street,
                                      City = user.City,
                                      State = user.State,
                                      ZipCode = user.ZipCode
                                  }).ToList();
            if (rm.IsAscending == true)
            {
                allData = rm.SortedColumnEmail switch
                {
                    "FirstName" => allData.OrderBy(x => x.FirstName).ToList(),
                    "LastName" => allData.OrderBy(x => x.LastName).ToList(),
                    "Email" => allData.OrderBy(x => x.Email).ToList(),
                    _ => allData.OrderBy(x => x.FirstName).ToList()
                };
            }
            else
            {
                allData = rm.SortedColumnEmail switch
                {
                    "FirstName" => allData.OrderByDescending(x => x.FirstName).ToList(),
                    "LastName" => allData.OrderByDescending(x => x.LastName).ToList(),
                    "Email" => allData.OrderByDescending(x => x.Email).ToList(),
                    _ => allData.OrderByDescending(x => x.FirstName).ToList()
                };
            }
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<User> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                Users = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
                IsAscending = rm.IsAscending,
                SortedColumnPHistory = rm.SortedColumnPHistory,
                PhoneNumber = rm.PhoneNumber,
                FirstName = rm.FirstName,
                LastName = rm.LastName,
                Email = rm.Email
            };
            return records;
        }
        #endregion

        #region PatientRecord
        public PaginatedViewModel PatientRecord(int UserId, PaginatedViewModel data)
        {
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                      join reqClient in _context.RequestClients
                                                      on req.RequestId equals reqClient.RequestId into reqClientGroup
                                                      from rc in reqClientGroup.DefaultIfEmpty()
                                                      join phys in _context.Physicians
                                                      on req.PhysicianId equals phys.PhysicianId into physGroup
                                                      from p in physGroup.DefaultIfEmpty()
                                                      join reg in _context.Regions
                                                      on rc.RegionId equals reg.RegionId into RegGroup
                                                      from rg in RegGroup.DefaultIfEmpty()
                                                      join encounter in _context.Encounters
                                                      on req.RequestId equals encounter.RequestId into reqEncounter
                                                      from enc in reqEncounter.DefaultIfEmpty()
                                                      where req.UserId == (UserId == null ? data.UserId : UserId)
                                                      select new AdminDashboardList
                                                      {
                                                          ProviderName = p.FirstName + " " + p.LastName,
                                                          RequestClientId = rc.RequestClientId,
                                                          Status = (short)req.Status,
                                                          RequestId = req.RequestId,
                                                          RequestTypeId = req.RequestTypeId,
                                                          Requestor = rc.FirstName + " " + rc.LastName,
                                                          PatientName = req.FirstName + " " + req.LastName,
                                                          RequestedDate = req.CreatedDate,
                                                          PatientPhoneNumber = rc.PhoneNumber,
                                                          Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                          Notes = rc.Notes,
                                                          ProviderId = req.PhysicianId,
                                                          RegionId = (int)rc.RegionId,
                                                          RequestorPhoneNumber = req.PhoneNumber,
                                                          ConcludedDate = req.CreatedDate,
                                                          ConfirmationNumber = req.ConfirmationNumber,
                                                          IsFinalized = enc.IsFinalized
                                                      }).ToList();
            if (data.IsAscending == true)
            {
                allData = data.SortedColumnPRecords switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "RequestedDate" => allData.OrderBy(x => x.RequestedDate).ToList(),
                    "ProviderName" => allData.OrderBy(x => x.ProviderName).ToList(),
                    "Status" => allData.OrderBy(x => x.Status).ToList(),
                    _ => allData.OrderBy(x => x.PatientName).ToList()
                };
            }
            else
            {
                allData = data.SortedColumnPRecords switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "RequestedDate" => allData.OrderByDescending(x => x.RequestedDate).ToList(),
                    "ProviderName" => allData.OrderByDescending(x => x.ProviderName).ToList(),
                    "Status" => allData.OrderByDescending(x => x.Status).ToList(),
                    _ => allData.OrderByDescending(x => x.PatientName).ToList()
                };
            }
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> result = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();

            PaginatedViewModel model = new()
            {
                UserId = UserId,
                adl = result,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                IsAscending = data.IsAscending,
                SortedColumnPRecords = data.SortedColumnPRecords

            };
            return model;
        }
        #endregion

        #region GetFilteredEmailLogs
        public RecordsModel GetFilteredEmailLogs(RecordsModel rm)
        {
            List<EmailLogs> allData = (from req in _context.EmailLogs
                                       where (rm.AccountType == null || rm.AccountType == 0 || req.RoleId == rm.AccountType) &&
                                             (!rm.StartDate.HasValue || req.CreateDate.Date == rm.StartDate.Value.Date) &&
                                             (!rm.EndDate.HasValue || req.SentDate.Value.Date == rm.EndDate.Value.Date) &&
                                             (rm.ReceiverName.IsNullOrEmpty() || _context.AspNetUsers.FirstOrDefault(e => e.Email == req.EmailId).UserName.ToLower().Contains(rm.ReceiverName.ToLower())) &&
                                             (rm.Email.IsNullOrEmpty() || req.EmailId.ToLower().Contains(rm.Email.ToLower()))
                                       select new EmailLogs
                                       {
                                           Recipient = _context.AspNetUsers.FirstOrDefault(e => e.Email == req.EmailId).UserName ?? null,
                                           ConfirmationNumber = req.ConfirmationNumber,
                                           CreateDate = req.CreateDate,
                                           EmailTemplate = req.EmailTemplate,
                                           FilePath = req.FilePath,
                                           SentDate = (DateTime)req.SentDate,
                                           RoleId = req.RoleId,
                                           EmailId = req.EmailId,
                                           SentTries = req.SentTries,
                                           SubjectName = req.SubjectName,
                                           Action = (int)req.Action
                                       }).ToList();
            if (rm.IsAscending == true)
            {
                allData = rm.SortedColumnEmail switch
                {
                    "Recipient" => allData.OrderBy(x => x.Recipient).ToList(),
                    "RoleId" => allData.OrderBy(x => x.RoleId).ToList(),
                    "EmailId" => allData.OrderBy(x => x.EmailId).ToList(),
                    "CreateDate" => allData.OrderBy(x => x.CreateDate).ToList(),
                    "SentTries" => allData.OrderBy(x => x.SentTries).ToList(),
                    _ => allData.OrderBy(x => x.Recipient).ToList()
                };
            }
            else
            {
                allData = rm.SortedColumnEmail switch
                {
                    "Recipient" => allData.OrderByDescending(x => x.Recipient).ToList(),
                    "RoleId" => allData.OrderByDescending(x => x.RoleId).ToList(),
                    "EmailId" => allData.OrderByDescending(x => x.EmailId).ToList(),
                    "CreateDate" => allData.OrderByDescending(x => x.CreateDate).ToList(),
                    "SentTries" => allData.OrderByDescending(x => x.SentTries).ToList(),
                    _ => allData.OrderByDescending(x => x.Recipient).ToList()
                };
            }
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogs> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                EmailLogs = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
                IsAscending = rm.IsAscending,
                SortedColumnEmail = rm.SortedColumnEmail,
                AccountType = rm.AccountType,
                StartDate = rm.StartDate,
                EndDate = rm.EndDate,
                ReceiverName = rm.ReceiverName,
                Email = rm.Email
            };

            return records;
        }
        #endregion

        #region GetFilteredSMSLogs
        public RecordsModel GetFilteredSMSLogs(RecordsModel rm)
        {
            List<SMSLogs> allData = (from req in _context.Smslogs
                                     where (rm.AccountType == null || rm.AccountType == 0 || req.RoleId == rm.AccountType) &&
                                           (!rm.StartDate.HasValue || req.CreateDate.Date == rm.StartDate.Value.Date) &&
                                           (!rm.EndDate.HasValue || req.SentDate.Value.Date == rm.EndDate.Value.Date) &&
                                           (rm.ReceiverName.IsNullOrEmpty() || _context.AspNetUsers.FirstOrDefault(e => e.PhoneNumber == req.MobileNumber).UserName.ToLower().Contains(rm.ReceiverName.ToLower())) &&
                                           (rm.PhoneNumber.IsNullOrEmpty() || req.MobileNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                     select new SMSLogs
                                     {
                                         Recipient = _context.AspNetUsers.FirstOrDefault(e => e.PhoneNumber == req.MobileNumber).UserName,
                                         ConfirmatioNumber = req.ConfirmationNumber,
                                         CreateDate = req.CreateDate,
                                         SmsTemplate = req.Smstemplate,
                                         SentDate = (DateTime)req.SentDate,
                                         RoleId = req.RoleId,
                                         MobileNumber = req.MobileNumber,
                                         SentTries = req.SentTries,
                                         Action = req.Action
                                     }).ToList();
            if (rm.IsAscending == true)
            {
                allData = rm.SortedColumnEmail switch
                {
                    "Recipient" => allData.OrderBy(x => x.Recipient).ToList(),
                    "RoleId" => allData.OrderBy(x => x.RoleId).ToList(),
                    "CreateDate" => allData.OrderBy(x => x.CreateDate).ToList(),
                    "SentTries" => allData.OrderBy(x => x.SentTries).ToList(),
                    _ => allData.OrderBy(x => x.Recipient).ToList()
                };
            }
            else
            {
                allData = rm.SortedColumnEmail switch
                {
                    "Recipient" => allData.OrderByDescending(x => x.Recipient).ToList(),
                    "RoleId" => allData.OrderByDescending(x => x.RoleId).ToList(),
                    "CreateDate" => allData.OrderByDescending(x => x.CreateDate).ToList(),
                    "SentTries" => allData.OrderByDescending(x => x.SentTries).ToList(),
                    _ => allData.OrderByDescending(x => x.Recipient).ToList()
                };
            }
            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SMSLogs> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                SMSLogs = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
                IsAscending = rm.IsAscending,
                SortedColumnEmail = rm.SortedColumnEmail,
                AccountType = rm.AccountType,
                StartDate = rm.StartDate,
                EndDate = rm.EndDate,
                ReceiverName = rm.ReceiverName,
                PhoneNumber = rm.PhoneNumber
            };
            return records;
        }
        #endregion
    }
}
