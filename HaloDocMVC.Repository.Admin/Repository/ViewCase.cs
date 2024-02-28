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
    public class ViewCase : IViewCase
    {
        private readonly HaloDocContext _context;
        public ViewCase(HaloDocContext context)
        {
            _context = context;
        }
        public ViewDataViewCase NewRequestData(int? RId, int? RTId)
        {
            ViewDataViewCase caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewDataViewCase()
                                        {
                                            UserId = req.Request.UserId,
                                            RequestId = (int)RId,
                                            RequestTypeId = (int)RTId,
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

        public ViewDataViewCase Edit(ViewDataViewCase vdvc, int? RId, int? RTId)
        {
            try
            {
                RequestClient RC = _context.RequestClients.FirstOrDefault(E => E.RequestId == vdvc.RequestId);

                RC.FirstName = vdvc.FirstName;
                RC.LastName = vdvc.LastName;
                RC.PhoneNumber = vdvc.Mobile;
                RC.Email = vdvc.Email;
                RC.Address = vdvc.Address;
                RC.NotiMobile = RC.PhoneNumber;
                RC.NotiEmail = RC.Email;
                RC.Notes = vdvc.Notes;
                RC.IntDate = vdvc.DOB.Day;
                RC.IntYear = vdvc.DOB.Year;
                RC.StrMonth = vdvc.DOB.Month.ToString();
                string[] SubAdd = vdvc.Address.Split(',');
                RC.Street = SubAdd[0];
                RC.City = SubAdd[1];
                RC.State = SubAdd[2];
                RC.ZipCode = SubAdd[3];
                _context.Update(RC);
                _context.SaveChanges();

                ViewDataViewCase caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewDataViewCase()
                                        {
                                            UserId = req.Request.UserId,
                                            RequestId = (int)RId,
                                            RequestTypeId = (int)RTId,
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
    }
}
