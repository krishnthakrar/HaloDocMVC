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
using Org.BouncyCastle.Ocsp;
using static HaloDocMVC.Entity.Models.ViewDataViewDocuments;

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
        public DashboardList GetPatientRequest(string id, DashboardList listdata)
        {
            List<DashboardList> allData = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == Int32.Parse(id)).Select(x => new DashboardList
            {
                createdDate = x.CreatedDate,
                Status = (short)x.Status,
                RequestId = x.RequestId,
                Fcount = x.RequestWiseFiles.Count()
            }).ToList();
            if (listdata.IsAscending == true)
            {
                allData = listdata.SortedColumn switch
                {
                    "createdDate" => allData.OrderBy(x => x.createdDate).ToList(),
                    "Status" => allData.OrderBy(x => x.Status).ToList(),
                    _ => allData.OrderBy(x => x.createdDate).ToList()
                };
            }
            else
            {
                allData = listdata.SortedColumn switch
                {
                    "createdDate" => allData.OrderByDescending(x => x.createdDate).ToList(),
                    "Status" => allData.OrderByDescending(x => x.Status).ToList(),
                    _ => allData.OrderByDescending(x => x.createdDate).ToList()
                };
            }
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)listdata.PageSize);
            List<DashboardList> list1 = allData.Skip((listdata.CurrentPage - 1) * listdata.PageSize).Take(listdata.PageSize).ToList();
            DashboardList Data = new DashboardList
            {
                patientdata = list1,
                CurrentPage = listdata.CurrentPage,
                TotalPages = totalPages,
                PageSize = listdata.PageSize,
                IsAscending = listdata.IsAscending,
                SortedColumn = listdata.SortedColumn,
                UserId = Int32.Parse(id)
            };
            return Data;
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
        public async Task<ViewDocument> ViewDocumentList(int? id, ViewDocument viewDocument)
        {
            var req = _context.RequestClients.FirstOrDefault(r => r.RequestId == id);
            var items = _context.RequestWiseFiles.Include(m => m.Request)
                .Where(x => x.RequestId == id).Select(m => new ViewDocument
                {
                    CreatedDate = m.CreatedDate,
                    RFirstName = m.Request.FirstName,
                    FileName = m.FileName
                }).ToList();
            if (viewDocument.IsAscending == true)
            {
                items = viewDocument.SortedColumn switch
                {
                    "CreatedDate" => items.OrderBy(x => x.CreatedDate).ToList(),
                    "FileName" => items.OrderBy(x => x.FileName).ToList(),
                    _ => items.OrderBy(x => x.CreatedDate).ToList()
                };
            }
            else
            {
                items = viewDocument.SortedColumn switch
                {
                    "CreatedDate" => items.OrderByDescending(x => x.CreatedDate).ToList(),
                    "FileName" => items.OrderByDescending(x => x.FileName).ToList(),
                    _ => items.OrderByDescending(x => x.CreatedDate).ToList()
                };
            }
            int totalItemCount = items.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)viewDocument.PageSize);
            List<ViewDocument> list1 = items.Skip((viewDocument.CurrentPage - 1) * viewDocument.PageSize).Take(viewDocument.PageSize).ToList();
            ViewDocument vd = new()
            {
                Files = list1,
                CurrentPage = viewDocument.CurrentPage,
                TotalPages = totalPages,
                PageSize = viewDocument.PageSize,
                IsAscending = viewDocument.IsAscending,
                SortedColumn = viewDocument.SortedColumn,
                RFirstName = req.FirstName,
                RequestId = req.RequestId
            };
            return vd;
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
