using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Buttons : IButtons
    {
        private readonly HaloDocContext _context;
        private readonly EmailConfiguration _emailConfig;

        public Buttons(HaloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }

        #region CreateRequest
        public void CreateRequest(ViewDataCreatePatient vdcp)
        {
            AspNetUser A = new();
            User U = new();
            Entity.DataModels.Request R = new();
            RequestClient RC = new();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcp.Email);
            var statename = _context.Regions.FirstOrDefault(x => x.RegionId == vdcp.State);
            if (isexist == null)
            {
                //AspNetUser Table
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = vdcp.UserName;
                A.PasswordHash = vdcp.PassWord;
                A.Email = vdcp.Email;
                A.PhoneNumber = vdcp.Mobile;
                A.CreatedDate = DateTime.Now;
                _context.Add(A);
                _context.SaveChanges();
                //User Table
                U.AspNetUserId = A.Id;
                U.FirstName = vdcp.FirstName;
                U.LastName = vdcp.LastName;
                U.Email = vdcp.Email;
                U.Mobile = vdcp.Mobile;
                U.Street = vdcp.Street;
                U.City = vdcp.City;
                U.RegionId = vdcp.State;
                U.State = statename.Name;
                U.ZipCode = vdcp.ZipCode;
                U.StrMonth = (vdcp.DOB.Month).ToString();
                U.IntDate = vdcp.DOB.Day;
                U.IntYear = vdcp.DOB.Year;
                U.CreatedBy = A.Id;
                U.CreatedDate = DateTime.Now;
                _context.Add(U);
                _context.SaveChanges();
            }

            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            U.Status = 1;
            //Request Table
            R.RequestTypeId = 2; // 2 stands for patient in RequestType table
            R.CreatedDate = DateTime.Now;
            R.FirstName = vdcp.FirstName;
            R.LastName = vdcp.LastName;
            R.Email = vdcp.Email;
            R.PhoneNumber = vdcp.Mobile;
            R.Status = U.Status;
            R.ConfirmationNumber = R.PhoneNumber;
            R.CreatedUserId = U.UserId;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.FirstName = vdcp.FirstName;
            RC.LastName = vdcp.LastName;
            RC.PhoneNumber = vdcp.Mobile;
            RC.Address = vdcp.Street + ", " + vdcp.City + ", " + vdcp.State + ", " + vdcp.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcp.Notes;
            RC.Email = vdcp.Email;
            RC.StrMonth = (vdcp.DOB.Month).ToString();
            RC.IntDate = vdcp.DOB.Day;
            RC.IntYear = vdcp.DOB.Year;
            RC.Street = vdcp.Street;
            RC.City = vdcp.City;
            RC.RegionId = vdcp.State;
            RC.State = statename.Name;
            RC.ZipCode = vdcp.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();
        }
        #endregion

        #region SendLink
        public Boolean SendLink(string FirstName, string LastName, string Email)
        {
            var agreementUrl = "https://localhost:44348/PatientHome/RequestLanding?Name=" + FirstName + " " + LastName + "&Email=" + Email;
            _emailConfig.SendMail(Email, "Link to Request", $"<a href='{agreementUrl}'>Request Page Link</a>");
            return true;
        }
        #endregion

        #region Export
        public List<AdminDashboardList> Export(string status)
        {
            List<int> statusdata = status.Split(',').Select(int.Parse).ToList();
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
        #endregion
    }
}
