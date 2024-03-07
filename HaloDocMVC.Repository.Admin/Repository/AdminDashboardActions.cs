using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class AdminDashboardActions : IAdminDashboardActions
    {
        private readonly HaloDocContext _context;
        public AdminDashboardActions(HaloDocContext context)
        {
            _context = context;
        }
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

        public async Task<bool> AssignProvider(int RequestId, int ProviderId, string notes)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = ProviderId;
            rsl.Notes = notes;

            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();

            return true;
        }

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
                        Status = 8,
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
                    BlockRequest blc = new BlockRequest
                    {
                        RequestId = requestData.RequestId,
                        PhoneNumber = requestData.PhoneNumber,
                        Email = requestData.Email,
                        Reason = Note,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.BlockRequests.Add(blc);
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
    }
}
