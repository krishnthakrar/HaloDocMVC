using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
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
        public List<AdminDashboardList> NewRequestData()
        {
            var list = _context.Requests.Join
                        (_context.RequestClients,
                        requestclients => requestclients.RequestId, requests => requests.RequestId,
                        (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                        )
                        .Where(req => req.Request.Status == 1)
                        .Select(req => new AdminDashboardList()
                        {
                            RequestId = req.Request.RequestId,
                            PatientName = req.Requestclient.FirstName + " " + req.Requestclient.LastName,
                            Email = req.Requestclient.Email,
                            DateOfBirth = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
                            RequestTypeId = req.Request.RequestTypeId,
                            Requestor = req.Request.FirstName + " " + req.Request.LastName,
                            RequestedDate = req.Request.CreatedDate,
                            PatientPhoneNumber = req.Requestclient.PhoneNumber,
                            RequestorPhoneNumber = req.Request.PhoneNumber,
                            Notes = req.Requestclient.Notes,
                            Address = req.Requestclient.Address
                        })
                        .OrderByDescending(req => req.RequestedDate)
                        .ToList();
            return list;
        }
        public CountStatusWiseRequestModel IndexData()
        {
            return new CountStatusWiseRequestModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => r.Status == 4 || r.Status == 5).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => r.Status == 3 || r.Status == 7 || r.Status == 8).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),
                adminDashboardList = NewRequestData()
            };
        }

        public List<AdminDashboardList> GetRequests(string Status)
        {

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
                                                where statusdata.Contains((int)req.Status)
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
            return allData;
        }
    }
}
