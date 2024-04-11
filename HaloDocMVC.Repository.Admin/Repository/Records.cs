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

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SearchRecords> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                SearchRecords = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
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
                                        where (!rm.StartDate.HasValue || req.CreatedDate.Value.Date == rm.StartDate.Value.Date) &&
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

            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<BlockRequests> list = data.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel model = new()
            {
                BlockRequests = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
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

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<User> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsModel records = new()
            {
                Users = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize
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
                                                          ConfirmationNumber = req.ConfirmationNumber
                                                      }).ToList();

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
                SearchInput = data.SearchInput
            };
            return model;
        }
        #endregion
    }
}
