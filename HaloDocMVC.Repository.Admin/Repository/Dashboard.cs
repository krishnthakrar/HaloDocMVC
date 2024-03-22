using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static HaloDocMVC.Entity.Models.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Entity.DataModels;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Http;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Dashboard : IDashboard
    {
        private readonly HaloDocContext _context;
        public Dashboard(HaloDocContext context)
        {
            _context = context;
        }

        #region PatientDashboard
        public List<DashboardList> DashboardList(int? id)
        {
            var items = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == id).Select(x => new DashboardList
            {
                createdDate = x.CreatedDate,
                Status = (AdminDashStatus)x.Status,
                RequestId = x.RequestId,
                Fcount = x.RequestWiseFiles.Count()

            }).ToList();
            return items;
        }
        #endregion

        #region PatientProfile
        public ViewDataUserProfile UserProfile(int id)
        {
            var UsersProfile = _context.Users
                                .Where(r => r.UserId == id)
                                .Select(r => new ViewDataUserProfile
                                {
                                    UserId = r.UserId,
                                    FirstName = r.FirstName,
                                    LastName = r.LastName,
                                    Mobile = r.Mobile,
                                    Email = r.Email,
                                    Street = r.Street,
                                    State = r.State,
                                    City = r.City,
                                    ZipCode = r.ZipCode,
                                    DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
                                })
                                .FirstOrDefault();

            return UsersProfile;
        }
        #endregion

        #region EditProfile
        public void EditProfile(ViewDataUserProfile vdup, int id)
        {
            User U = _context.Users.Find(id);

            U.FirstName = vdup.FirstName;
            U.LastName = vdup.LastName;
            U.Mobile = vdup.Mobile;
            U.Email = vdup.Email;
            U.State = vdup.State;
            U.Street = vdup.Street;
            U.City = vdup.City;
            U.ZipCode = vdup.ZipCode;
            U.IntDate = vdup.DOB.Day;
            U.IntYear = vdup.DOB.Year;
            U.StrMonth = vdup.DOB.Month.ToString();
            U.ModifiedBy = vdup.CreatedBy;
            U.ModifiedDate = DateTime.Now;
            _context.Update(U);
            _context.SaveChanges();
        }
        #endregion

        #region ViewDoc
        public List<ViewDocument> ViewDocumentList(int? id)
        {
            var items = _context.RequestWiseFiles.Include(m => m.Request)
                .Where(x => x.RequestId == id).Select(m => new ViewDocument
                {
                    CreatedDate = m.CreatedDate,
                    RFirstName = m.Request.FirstName,
                    FileName = m.FileName
                }).ToList();
            return items;
        }
        #endregion

        #region UploadDoc
        public void UploadDoc(int RequestId, IFormFile? UploadFile)
        {
            string UploadImage;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
                UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + UploadFile.FileName;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream)
;
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = RequestId,
                    FileName = UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
