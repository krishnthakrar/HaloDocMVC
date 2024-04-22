using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class PatientRequest : IPatientRequest
    {
        private readonly HaloDocContext _context;
        private readonly EmailConfiguration _emailConfig;

        public PatientRequest(HaloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }
        #region CreatePatient
        public void CreatePatient(ViewDataCreatePatient vdcp)
        {
            AspNetUser A = new();
            User U = new();
            Request R = new();
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

            if (vdcp.UploadFile != null)
            {
                string filePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, vdcp.UploadFile.FileName);
                vdcp.UploadImage = "~" + filePath.Replace("wwwroot\\", "/") + "/" + vdcp.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vdcp.UploadFile.CopyTo(stream);
                }
                RequestWiseFile RWF = new();
                RWF.RequestId = R.RequestId;
                RWF.FileName = vdcp.UploadFile.FileName;
                RWF.CreatedDate = DateTime.Now;
                _context.Add(RWF);
                _context.SaveChanges();
            }
        }
        #endregion

        #region CreateFriend
        public void CreateFriend(ViewDataCreateFriend vdcf)
        {
            Request R = new();
            RequestClient RC = new();

            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcf.pEmail);
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:44348/PatientHome/CreateAccount";
                _emailConfig.SendMail(vdcf.pEmail, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }
            //Request Table
            R.RequestTypeId = 3; // 3 stands for Family/Friend in RequestType table
            R.CreatedDate = DateTime.Now;
            R.FirstName = vdcf.fFirstName;
            R.LastName = vdcf.fLastName;
            R.Email = vdcf.fEmail;
            R.PhoneNumber = vdcf.PhoneNumber;
            R.Status = 1;
            R.ConfirmationNumber = R.PhoneNumber;
            R.RelationName = vdcf.RelationName;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.FirstName = vdcf.pFirstName;
            RC.LastName = vdcf.pLastName;
            RC.PhoneNumber = vdcf.Mobile;
            RC.Address = vdcf.Street + ", " + vdcf.City + ", " + vdcf.State + ", " + vdcf.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcf.Notes;
            RC.Email = vdcf.pEmail;
            RC.StrMonth = (vdcf.DOB.Month).ToString();
            RC.IntDate = vdcf.DOB.Day;
            RC.IntYear = vdcf.DOB.Year;
            RC.Street = vdcf.Street;
            RC.City = vdcf.City;
            RC.State = vdcf.State;
            RC.ZipCode = vdcf.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();

            if (vdcf.UploadFile != null)
            {
                string filePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, vdcf.UploadFile.FileName);
                vdcf.UploadImage = "~" + filePath.Replace("wwwroot\\", "/") + "/" + vdcf.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vdcf.UploadFile.CopyTo(stream);
                }
                RequestWiseFile RWF = new();
                RWF.RequestId = R.RequestId;
                RWF.FileName = vdcf.UploadFile.FileName;
                RWF.CreatedDate = DateTime.Now;
                _context.Add(RWF);
                _context.SaveChanges();
            }

        }
        #endregion

        #region CreateConcierge
        public void CreateConcierge(ViewDataCreateConcierge vdcc)
        {
            Request R = new();
            RequestClient RC = new();
            Concierge C = new();
            RequestConcierge RCO = new();

            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcc.pEmail);
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:44348/PatientHome/CreateAccount";
                _emailConfig.SendMail(vdcc.pEmail, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }
            //Request Table
            R.RequestTypeId = 4; // 4 stands for Concierge in RequestType table
            R.CreatedDate = DateTime.Now;
            R.FirstName = vdcc.cFirstName;
            R.LastName = vdcc.cLastName;
            R.Email = vdcc.cEmail;
            R.PhoneNumber = vdcc.PhoneNumber;
            R.Status = 1;
            R.ConfirmationNumber = R.PhoneNumber;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.FirstName = vdcc.pFirstName;
            RC.LastName = vdcc.pLastName;
            RC.PhoneNumber = vdcc.Mobile;
            RC.Address = vdcc.Street + ", " + vdcc.City + ", " + vdcc.State + ", " + vdcc.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcc.Notes;
            RC.Email = vdcc.pEmail;
            RC.StrMonth = (vdcc.DOB.Month).ToString();
            RC.IntDate = vdcc.DOB.Day;
            RC.IntYear = vdcc.DOB.Year;
            RC.Street = vdcc.Street;
            RC.City = vdcc.City;
            RC.State = vdcc.State;
            RC.ZipCode = vdcc.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();
            //Concierge Table
            C.ConciergeName = vdcc.cFirstName + " " + vdcc.cLastName;
            C.Address = vdcc.Room + ", " + vdcc.pName + ", " + vdcc.Street + ", " + vdcc.City + ", " + vdcc.State + ", " + vdcc.ZipCode;
            C.Street = vdcc.Street;
            C.City = vdcc.City;
            C.State = vdcc.State;
            C.ZipCode = vdcc.ZipCode;
            C.CreatedDate = DateTime.Now;
            _context.Add(C);
            _context.SaveChanges();
            //RequestConcierge Table
            RCO.RequestId = R.RequestId;
            RCO.ConciergeId = C.ConciergeId;
            _context.Add(RCO);
            _context.SaveChanges();
        }
        #endregion

        #region CreatePartner
        public void CreatePartner(ViewDataCreateBusiness vdcb)
        {
            Request R = new();
            RequestClient RC = new();
            Business B = new();
            RequestBusiness RB = new();

            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcb.pEmail);
            if (isexist == null)
            {
                var Subject = "Create Account";
                var agreementUrl = "https://localhost:44348/PatientHome/CreateAccount";
                _emailConfig.SendMail(vdcb.pEmail, Subject, $"<a href='{agreementUrl}'>Create Account</a>");
            }

            //Request Table
            R.RequestTypeId = 1; // 1 stands for Business in RequestType table
            R.CreatedDate = DateTime.Now;
            R.FirstName = vdcb.bFirstName;
            R.LastName = vdcb.bLastName;
            R.Email = vdcb.bEmail;
            R.PhoneNumber = vdcb.PhoneNumber;
            R.Status = 1;
            R.ConfirmationNumber = R.PhoneNumber;
            R.CaseNumber = vdcb.CaseNumber;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.FirstName = vdcb.pFirstName;
            RC.LastName = vdcb.pLastName;
            RC.PhoneNumber = vdcb.Mobile;
            RC.Address = vdcb.Street + ", " + vdcb.City + ", " + vdcb.State + ", " + vdcb.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcb.Notes;
            RC.Email = vdcb.pEmail;
            RC.StrMonth = (vdcb.DOB.Month).ToString();
            RC.IntDate = vdcb.DOB.Day;
            RC.IntYear = vdcb.DOB.Year;
            RC.Street = vdcb.Street;
            RC.City = vdcb.City;
            RC.State = vdcb.State;
            RC.ZipCode = vdcb.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();
            //Business Table
            B.Name = vdcb.Property;
            B.Address1 = vdcb.Property + ", " + vdcb.Street;
            B.Address2 = vdcb.City + ", " + vdcb.State + ", " + vdcb.ZipCode;
            B.City = vdcb.City;
            B.ZipCode = vdcb.ZipCode;
            B.PhoneNumber = vdcb.PhoneNumber;
            B.CreatedDate = DateTime.Now;
            _context.Add(B);
            _context.SaveChanges();
            //RequestBusiness Table
            RB.RequestId = R.RequestId;
            RB.BusinessId = B.BusinessId;
            _context.Add(RB);
            _context.SaveChanges();
        }
        #endregion

        #region CreateMeIndex
        public ViewDataCreatePatient ViewMe(int id)
        {
            var ViewMe = _context.Users.Where(r => (r.UserId) == id).Select(r => new ViewDataCreatePatient
            {
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Mobile = r.Mobile,
                DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
            }).FirstOrDefault();
            return ViewMe;
        }
        #endregion

        #region CreateMe
        public void CreateMe(ViewDataCreatePatient vdcp)
        {
            AspNetUser A = new();
            User U = new();
            Request R = new();
            RequestClient RC = new();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcp.Email);
            var statename = _context.Regions.FirstOrDefault(x => x.RegionId == vdcp.State);
            if (isexist == null)
            {
                //AspNetUser Table
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = vdcp.FirstName + vdcp.LastName;
                A.PasswordHash = vdcp.FirstName + vdcp.LastName;
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

            if (vdcp.UploadFile != null)
            {
                string filePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, vdcp.UploadFile.FileName);
                vdcp.UploadImage = "~" + filePath.Replace("wwwroot\\", "/") + "/" + vdcp.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vdcp.UploadFile.CopyTo(stream);
                }
                RequestWiseFile RWF = new();
                RWF.RequestId = R.RequestId;
                RWF.FileName = vdcp.UploadFile.FileName;
                RWF.CreatedDate = DateTime.Now;
                _context.Add(RWF);
                _context.SaveChanges();
            }
        }
        #endregion

        #region CreateSomeOneElse
        public void CreateSomeOneElse(ViewDataCreateSomeOneElse vdcs)
        {
            var R = new Request();
            var RC = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcs.Email);
            R.RequestTypeId = 3;
            R.FirstName = vdcs.FirstName;
            R.LastName = vdcs.LastName;
            R.Email = vdcs.Email;
            R.PhoneNumber = vdcs.Mobile;
            R.RelationName = vdcs.RelationName;
            R.CreatedDate = DateTime.Now;
            _context.Requests.Add(R);
            _context.SaveChanges();

            RC.RequestId = R.RequestId;
            RC.FirstName = vdcs.FirstName;
            RC.Address = vdcs.Street;
            RC.LastName = vdcs.LastName;
            RC.Email = vdcs.Email;
            RC.PhoneNumber = vdcs.Mobile;

            _context.RequestClients.Add(RC);
            _context.SaveChanges();


            if (vdcs.UploadFile != null)
            {
                string filePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, vdcs.UploadFile.FileName);
                vdcs.UploadImage = "~" + filePath.Replace("wwwroot\\", "/") + "/" + vdcs.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vdcs.UploadFile.CopyTo(stream);
                }
                RequestWiseFile RWF = new();
                RWF.RequestId = R.RequestId;
                RWF.FileName = vdcs.UploadFile.FileName;
                RWF.CreatedDate = DateTime.Now;
                _context.Add(RWF);
                _context.SaveChanges();
            }

        }
        #endregion
    }
}
