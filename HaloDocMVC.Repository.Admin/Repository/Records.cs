using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}
