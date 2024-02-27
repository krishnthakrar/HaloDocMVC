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
                            Address = req.Requestclient.Address + " " + req.Requestclient.Street + " " + req.Requestclient.City + " " + req.Requestclient.State
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
    }
}
