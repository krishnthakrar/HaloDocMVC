using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly HaloDocContext _context;
        public RequestRepository(HaloDocContext context)
        {
            _context = context;
        }

        #region IndexData
        public PaginatedViewModel IndexData(int ProviderId)
        {
            if (ProviderId < 0)
            {
                return new PaginatedViewModel
                {
                    NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                    PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                    ActiveRequest = _context.Requests.Where(r => r.Status == 4 || r.Status == 5).Count(),
                    ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                    ToCloseRequest = _context.Requests.Where(r => r.Status == 3 || r.Status == 7 || r.Status == 8).Count(),
                    UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),
                };
            }
            return new PaginatedViewModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1 && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1)).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2 && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1)).Count(),
                ActiveRequest = _context.Requests.Where((r => (r.Status == 4 || r.Status == 5) && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1))).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6 && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1)).Count(),
                ToCloseRequest = _context.Requests.Where((r => (r.Status == 3 || r.Status == 7 || r.Status == 8) && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1))).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9 && r.PhysicianId == ProviderId && r.IsDeleted == new BitArray(1)).Count()
            };
        }
        #endregion

        #region AdminDashboardIndex
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data)
        {
            if (data.SearchInput != null)
            {
                data.SearchInput = data.SearchInput.Trim();
            }
            List<int> statusdata = Status.Split(',').Select(int.Parse).ToList();
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
                                                where statusdata.Contains((int)req.Status) && (data.SearchInput == null ||
                                                         rc.FirstName.Contains(data.SearchInput) || rc.LastName.Contains(data.SearchInput) ||
                                                         req.FirstName.Contains(data.SearchInput) || req.LastName.Contains(data.SearchInput) ||
                                                         rc.Email.Contains(data.SearchInput) || rc.PhoneNumber.Contains(data.SearchInput) ||
                                                         rc.Address.Contains(data.SearchInput) || rc.Notes.Contains(data.SearchInput) ||
                                                         p.FirstName.Contains(data.SearchInput) || p.LastName.Contains(data.SearchInput) ||
                                                         rg.Name.Contains(data.SearchInput)) && (data.RegionId == null || rc.RegionId == data.RegionId)
                                                         && (data.RequestType == null || req.RequestTypeId == data.RequestType)
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.RequestId,
                                                    RequestTypeId = req.RequestTypeId,
                                                    Requestor = req.FirstName + " " + req.LastName,
                                                    PatientName = rc.FirstName + " " + rc.LastName,
                                                    DateOfBirth = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                                    RequestedDate = req.CreatedDate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PatientPhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address,
                                                    Notes = rc.Notes,
                                                    ProviderId = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            if (data.IsAscending == true)
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderBy(x => x.Requestor).ToList(),
                    "DateOfBirth" => allData.OrderBy(x => x.DateOfBirth).ToList(),
                    "Address" => allData.OrderBy(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderBy(x => x.RequestedDate).ToList(),
                    _ => allData.OrderBy(x => x.RequestedDate).ToList()
                };
            }
            else
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderByDescending(x => x.Requestor).ToList(),
                    "DateOfBirth" => allData.OrderByDescending(x => x.DateOfBirth).ToList(),
                    "Address" => allData.OrderByDescending(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderByDescending(x => x.RequestedDate).ToList(),
                    _ => allData.OrderByDescending(x => x.RequestedDate).ToList()
                };
            }
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            PaginatedViewModel paginatedViewModel = new()
            {
                adl = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput,
                IsAscending = data.IsAscending,
                SortedColumn = data.SortedColumn
            };
            return paginatedViewModel;
        }
        #endregion

        #region ProviderDashboardIndex
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data, int ProviderId)
        {
            if (data.SearchInput != null)
            {
                data.SearchInput = data.SearchInput.Trim();
            }
            List<int> statusdata = Status.Split(',').Select(int.Parse).ToList();
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
                                                where statusdata.Contains((int)req.Status) && (data.SearchInput == null ||
                                                         rc.FirstName.Contains(data.SearchInput) || rc.LastName.Contains(data.SearchInput) ||
                                                         req.FirstName.Contains(data.SearchInput) || req.LastName.Contains(data.SearchInput) ||
                                                         rc.Email.Contains(data.SearchInput) || rc.PhoneNumber.Contains(data.SearchInput) ||
                                                         rc.Address.Contains(data.SearchInput) || rc.Notes.Contains(data.SearchInput) ||
                                                         p.FirstName.Contains(data.SearchInput) || p.LastName.Contains(data.SearchInput) ||
                                                         rg.Name.Contains(data.SearchInput)) && (data.RegionId == null || rc.RegionId == data.RegionId)
                                                         && (data.RequestType == null || req.RequestTypeId == data.RequestType) && req.PhysicianId == ProviderId
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.RequestId,
                                                    RequestTypeId = req.RequestTypeId,
                                                    Requestor = req.FirstName + " " + req.LastName,
                                                    PatientName = rc.FirstName + " " + rc.LastName,
                                                    DateOfBirth = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                                    RequestedDate = req.CreatedDate,
                                                    Email = rc.Email,
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PatientPhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address,
                                                    Notes = rc.Notes,
                                                    ProviderId = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            if (data.IsAscending == true)
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "Address" => allData.OrderBy(x => x.Address).ToList(),
                    _ => allData.OrderBy(x => x.PatientName).ToList()
                };
            }
            else
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "Address" => allData.OrderByDescending(x => x.Address).ToList(),
                    _ => allData.OrderByDescending(x => x.PatientName).ToList()
                };
            }
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            PaginatedViewModel paginatedViewModel = new()
            {
                adl = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput,
                IsAscending = data.IsAscending,
                SortedColumn = data.SortedColumn
            };
            return paginatedViewModel;
        }
        #endregion
    }
}
