﻿using HaloDocMVC.Entity.DataContext;
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
            vdvn.PatientNotes = symptoms.Notes;
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
    }
}
