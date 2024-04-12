using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static HaloDocMVC.Entity.Models.ProviderMenu;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Providers : IProviders
    {
        private readonly HaloDocContext _context;
        private readonly EmailConfiguration _emailConfig;

        public Providers(HaloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }

        #region PhysicianAll
        public ProviderMenu PhysicianAll(ProviderMenu pm)
        {
            List<ProviderMenu>? data = (from r in _context.Physicians
                                        join Notifications in _context.PhysicianNotifications
                                        on r.PhysicianId equals Notifications.PhysicianId into aspGroup
                                        from nof in aspGroup.DefaultIfEmpty()
                                        join role in _context.Roles
                                        on r.RoleId equals role.RoleId into roleGroup
                                        from roles in roleGroup.DefaultIfEmpty()
                                        where r.IsDeleted == new BitArray(1)
                                        select new ProviderMenu
                                        {
                                            NotificationId = nof.Id,
                                            CreatedDate = r.CreatedDate,
                                            PhysicianId = r.PhysicianId,
                                            Address1 = r.Address1,
                                            Address2 = r.Address2,
                                            AdminNotes = r.AdminNotes,
                                            AltPhone = r.AltPhone,
                                            BusinessName = r.BusinessName,
                                            BusinessWebsite = r.BusinessWebsite,
                                            City = r.City,
                                            Name = r.FirstName + " " + r.LastName,
                                            Notification = nof.IsNotificationStopped,
                                            Role = roles.Name,
                                            Status = r.Status,
                                            Email = r.Email,
                                            IsNonDisclosureDoc = r.IsNonDisclosureDoc == null ? false : true
                                        }).ToList();
            if (pm.IsAscending == true)
            {
                data = pm.SortedColumn switch
                {
                    "PatientName" => data.OrderBy(x => x.Name).ToList(),
                    "Role" => data.OrderBy(x => x.Role).ToList(),
                    _ => data.OrderBy(x => x.Name).ToList()
                };
            }
            else
            {
                data = pm.SortedColumn switch
                {
                    "PatientName" => data.OrderByDescending(x => x.Name).ToList(),
                    "Role" => data.OrderByDescending(x => x.Role).ToList(),
                    _ => data.OrderByDescending(x => x.Name).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pm.PageSize);
            List<ProviderMenu> list = data.Skip((pm.CurrentPage - 1) * pm.PageSize).Take(pm.PageSize).ToList();

            ProviderMenu model = new()
            {
                ProviderData = list,
                CurrentPage = pm.CurrentPage,
                TotalPages = totalPages,
                PageSize = pm.PageSize,
                IsAscending = pm.IsAscending,
                SortedColumn = pm.SortedColumn
            };

            return model;
        }
        #endregion

        #region ChangeNotificationPhysician
        public bool ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict)
        {
            try
            {
                if (changedValuesDict == null)
                {
                    return false;
                }
                else
                {
                    foreach (var item in changedValuesDict)
                    {
                        var ar = _context.PhysicianNotifications.Where(r => r.PhysicianId == item.Key).FirstOrDefault();
                        if (ar != null)
                        {
                            ar.IsNotificationStopped[0] = item.Value;
                            _context.PhysicianNotifications.Update(ar);
                            _context.SaveChanges();
                        }
                        else
                        {
                            PhysicianNotification pn = new();
                            pn.PhysicianId = item.Key;
                            pn.IsNotificationStopped = new BitArray(1);
                            pn.IsNotificationStopped[0] = item.Value;
                            _context.PhysicianNotifications.Add(pn);
                            _context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region PhysicianByRegion
        public ProviderMenu PhysicianByRegion(ProviderMenu pm, int? region)
        {
            List<ProviderMenu> data = (from pr in _context.PhysicianRegions
                                       join ph in _context.Physicians
                                       on pr.PhysicianId equals ph.PhysicianId into rGroup
                                       from r in rGroup.DefaultIfEmpty()
                                       join Notifications in _context.PhysicianNotifications
                                       on r.PhysicianId equals Notifications.PhysicianId into aspGroup
                                       from nof in aspGroup.DefaultIfEmpty()
                                       join role in _context.Roles
                                       on r.RoleId equals role.RoleId into roleGroup
                                       from roles in roleGroup.DefaultIfEmpty()
                                       where pr.RegionId == region
                                       select new ProviderMenu
                                       {
                                           CreatedDate = r.CreatedDate,
                                           PhysicianId = r.PhysicianId,
                                           Address1 = r.Address1,
                                           Address2 = r.Address2,
                                           AdminNotes = r.AdminNotes,
                                           AltPhone = r.AltPhone,
                                           BusinessName = r.BusinessName,
                                           BusinessWebsite = r.BusinessWebsite,
                                           City = r.City,
                                           Name = r.FirstName + " " + r.LastName,
                                           Notification = nof.IsNotificationStopped,
                                           Role = roles.Name,
                                           Status = r.Status,
                                           Email = r.Email,
                                           IsNonDisclosureDoc = r.IsNonDisclosureDoc == null ? false : true
                                       }).ToList();
            if (pm.IsAscending == true)
            {
                data = pm.SortedColumn switch
                {
                    "PatientName" => data.OrderBy(x => x.Name).ToList(),
                    _ => data.OrderBy(x => x.Name).ToList()
                };
            }
            else
            {
                data = pm.SortedColumn switch
                {
                    "PatientName" => data.OrderByDescending(x => x.Name).ToList(),
                    _ => data.OrderByDescending(x => x.Name).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pm.PageSize);
            List<ProviderMenu> list = data.Skip((pm.CurrentPage - 1) * pm.PageSize).Take(pm.PageSize).ToList();

            ProviderMenu model = new()
            {
                ProviderData = list,
                CurrentPage = pm.CurrentPage,
                TotalPages = totalPages,
                PageSize = pm.PageSize,
                RegionId = pm.RegionId,
                IsAscending = pm.IsAscending,
                SortedColumn = pm.SortedColumn
            };

            return model;
        }
        #endregion

        #region GetProfileDetails
        public ProviderMenu GetProfileDetails(int UserId)
        {
            ProviderMenu? v = (from r in _context.Physicians
                               join Aspnetuser in _context.AspNetUsers
                               on r.AspNetUserId equals Aspnetuser.Id into aspGroup
                               from asp in aspGroup.DefaultIfEmpty()
                               where r.PhysicianId == UserId
                               select new ProviderMenu
                               {
                                   RoleId = r.RoleId,
                                   PhysicianId = r.PhysicianId,
                                   UserName = r.FirstName + " " + r.LastName,
                                   Address1 = r.Address1,
                                   Address2 = r.Address2,
                                   AltPhone = r.AltPhone,
                                   City = r.City,
                                   AspNetUserId = r.AspNetUserId,
                                   CreatedBy = r.CreatedBy,
                                   Email = r.Email,
                                   CreatedDate = r.CreatedDate,
                                   Mobile = r.Mobile,
                                   ModifiedBy = r.ModifiedBy,
                                   ModifiedDate = r.ModifiedDate,
                                   RegionId = r.RegionId,
                                   LastName = r.LastName,
                                   FirstName = r.FirstName,
                                   Status = r.Status,
                                   ZipCode = r.Zip,
                                   MedicalLicense = r.MedicalLicense,
                                   NpiNumber = r.Npinumber,
                                   SyncEmailAddress = r.SyncEmailAddress,
                                   BusinessName = r.BusinessName,
                                   BusinessWebsite = r.BusinessWebsite,
                                   AdminNotes = r.AdminNotes,
                                   IsAgreementDoc = r.IsAgreementDoc[0],
                                   IsNonDisclosureDoc = r.IsNonDisclosureDoc == false ? false : true,
                                   IsBackgroundDoc = r.IsBackgroundDoc[0],
                                   IsLicenseDoc = r.IsLicenseDoc[0],
                                   IsTrainingDoc = r.IsTrainingDoc[0],
                               }).FirstOrDefault();
            List<Regions> regions = new();
            regions = _context.PhysicianRegions
                  .Where(r => r.PhysicianId == UserId)
                  .Select(req => new Regions()
                  {
                      RegionId = req.RegionId
                  }).ToList();
            v.RegionIds = regions;
            return v;
        }
        #endregion

        #region EditPassword
        public bool EditPassword(string password, ProviderMenu pm)
        {
            var req = _context.Physicians.Where(W => W.PhysicianId == pm.PhysicianId).FirstOrDefault();
            if (req != null)
            {
                /*req.FirstName = pm.FirstName;
                req.LastName = pm.LastName;*/
                req.Status = pm.Status;
                req.RoleId = pm.RoleId;
                _context.Physicians.Update(req);
                _context.SaveChanges();
                return true;
            }
            AspNetUser? U = _context.AspNetUsers.FirstOrDefault(m => m.Id == req.AspNetUserId);
            if (U != null)
            {
                U.PasswordHash = password;
                _context.AspNetUsers.Update(U);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region EditPhysInfo
        public bool EditPhysInfo(ProviderMenu pm)
        {
            try
            {
                if (pm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Physicians.Where(W => W.PhysicianId == pm.PhysicianId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = pm.Email;
                        DataForChange.FirstName = pm.FirstName;
                        DataForChange.LastName = pm.LastName;
                        DataForChange.Mobile = pm.Mobile;
                        DataForChange.MedicalLicense = pm.MedicalLicense;
                        DataForChange.Npinumber = pm.NpiNumber;
                        DataForChange.SyncEmailAddress = pm.SyncEmailAddress;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = _context.PhysicianRegions.Where(r => r.PhysicianId == pm.PhysicianId).Select(req => req.RegionId).ToList();
                        List<int> priceList = pm.RegionsId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                PhysicianRegion pr = new()
                                {
                                    RegionId = item,
                                    PhysicianId = (int)pm.PhysicianId
                                };
                                _context.PhysicianRegions.Update(pr);
                                _context.SaveChanges();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                PhysicianRegion pr = _context.PhysicianRegions.Where(r => r.PhysicianId == pm.PhysicianId && r.RegionId == item).First();
                                _context.PhysicianRegions.Remove(pr);
                                _context.SaveChanges();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region BillingInfoEdit
        public bool BillingInfoEdit(ProviderMenu pm)
        {
            try
            {
                if (pm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Physicians.Where(W => W.PhysicianId == pm.PhysicianId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = pm.Address1;
                        DataForChange.Address2 = pm.Address2;
                        DataForChange.City = pm.City;
                        DataForChange.Mobile = pm.Mobile;
                        DataForChange.Zip = pm.ZipCode;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region ProviderInfoEdit
        public bool ProviderInfoEdit(ProviderMenu pm)
        {
            try
            {
                if (pm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Physicians.Where(W => W.PhysicianId == pm.PhysicianId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.BusinessName = pm.BusinessName;
                        DataForChange.BusinessWebsite = pm.BusinessWebsite;
                        DataForChange.AdminNotes = pm.AdminNotes;
                        int id = (int)pm.PhysicianId;
                        if (pm.PhotoFile != null)
                        {
                            string UploadPhoto = SaveFile.UploadDoc(pm.PhotoFile, id);
                            DataForChange.Photo = UploadPhoto;
                        }
                        if (pm.SignatureFile != null)
                        {
                            string UploadSign = SaveFile.UploadDoc(pm.SignatureFile, id);
                            DataForChange.Signature = UploadSign;
                        }
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region ProviderEditSubmit
        public bool ProviderEditSubmit(ProviderMenu pm)
        {
            try
            {
                if (pm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Physicians.Where(W => W.PhysicianId == pm.PhysicianId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        SaveFile.UploadProviderDoc(pm.AgreementDoc, (int)pm.PhysicianId, "Agreementdoc.pdf");
                        SaveFile.UploadProviderDoc(pm.BackGroundDoc, (int)pm.PhysicianId, "BackGrounddoc.pdf");
                        SaveFile.UploadProviderDoc(pm.NonDisclosureDoc, (int)pm.PhysicianId, "NonDisclosuredoc.pdf");
                        SaveFile.UploadProviderDoc(pm.LicenseDoc, (int)pm.PhysicianId, "Agreementdoc.pdf");
                        SaveFile.UploadProviderDoc(pm.TrainingDoc, (int)pm.PhysicianId, "Trainingdoc.pdf");
                        DataForChange.IsAgreementDoc = new BitArray(1);
                        DataForChange.IsBackgroundDoc = new BitArray(1);
                        DataForChange.IsNonDisclosureDoc = false;
                        DataForChange.IsLicenseDoc = new BitArray(1);
                        DataForChange.IsTrainingDoc = new BitArray(1);

                        DataForChange.IsAgreementDoc[0] = pm.IsAgreementDoc;
                        DataForChange.IsBackgroundDoc[0] = pm.IsBackgroundDoc;
                        DataForChange.IsNonDisclosureDoc = pm.IsNonDisclosureDoc;
                        DataForChange.IsLicenseDoc[0] = pm.IsLicenseDoc;
                        DataForChange.IsTrainingDoc[0] = pm.IsTrainingDoc;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region CreateProvider
        public bool CreateProvider(ProviderMenu pm, string? id)
        {
            AspNetUser A = new();
            AspNetUserRole AUR = new();
            Physician p = new();
            var isexist = _context.Physicians.FirstOrDefault(x => x.Email == pm.Email);
            var asp = _context.AspNetUsers.FirstOrDefault(x => x.Email == pm.Email);
            if (isexist == null)
            {
                if (asp == null)
                {
                    //AspNetUser Table
                    Guid g = Guid.NewGuid();
                    A.Id = g.ToString();
                    A.UserName = pm.UserName;
                    A.PasswordHash = pm.PassWord;
                    A.Email = pm.Email;
                    A.PhoneNumber = pm.AltPhone;
                    A.CreatedDate = DateTime.Now;
                    _context.Add(A);
                    _context.SaveChanges();

                    AUR.UserId = A.Id;
                    AUR.RoleId = "2";
                    _context.Add(AUR);
                    _context.SaveChanges();

                    p.AspNetUserId = A.Id;
                }
                else
                {
                    p.AspNetUserId = asp.Id;
                }
                //Physician Table
                p.FirstName = pm.FirstName;
                p.LastName = pm.LastName;
                p.Email = pm.Email;
                p.Mobile = pm.Mobile;
                p.MedicalLicense = pm.MedicalLicense;

                p.AdminNotes = pm.AdminNotes;
                //isagreement
                //background
                p.Address1 = pm.Address1;
                p.Address2 = pm.Address2;
                p.City = pm.City;
                p.Zip = pm.ZipCode;
                p.AltPhone = pm.AltPhone;
                p.CreatedBy = "10d25ba5-69a7-4e20-85df-3e0d1c4f96l2";
                p.CreatedDate = DateTime.Now;
                p.Status = 1;
                p.BusinessName = pm.BusinessName;
                p.BusinessWebsite = pm.BusinessWebsite;
                p.IsDeleted = new BitArray(1);
                p.RoleId = pm.RoleId;
                p.Npinumber = pm.NpiNumber;
                p.IsAgreementDoc = new BitArray(1);
                p.IsBackgroundDoc = new BitArray(1);
                p.IsNonDisclosureDoc = false;
                p.IsLicenseDoc = new BitArray(1);
                p.IsTrainingDoc = new BitArray(1);

                p.IsAgreementDoc[0] = pm.IsAgreementDoc;
                p.IsBackgroundDoc[0] = pm.IsBackgroundDoc;
                p.IsNonDisclosureDoc = pm.IsNonDisclosureDoc;
                p.IsLicenseDoc[0] = pm.IsLicenseDoc;
                p.IsTrainingDoc[0] = pm.IsTrainingDoc;
                p.IsDeleted[0] = false;
                //credential
                p.IsNonDisclosureDoc = true;
                _context.Add(p);
                _context.SaveChanges();
                SaveFile.UploadProviderDoc(pm.AgreementDoc, p.PhysicianId, "Agreementdoc.pdf");
                SaveFile.UploadProviderDoc(pm.BackGroundDoc, p.PhysicianId, "BackGrounddoc.pdf");
                SaveFile.UploadProviderDoc(pm.NonDisclosureDoc, p.PhysicianId, "NonDisclosuredoc.pdf");
                SaveFile.UploadProviderDoc(pm.LicenseDoc, p.PhysicianId, "Agreementdoc.pdf");
                SaveFile.UploadProviderDoc(pm.TrainingDoc, p.PhysicianId, "Trainingdoc.pdf");

                SaveFile.UploadProviderDoc(pm.SignatureFile, p.PhysicianId, p.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png");
                SaveFile.UploadProviderDoc(pm.PhotoFile, p.PhysicianId, p.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(pm.PhotoFile.FileName).Trim('.'));

                if (pm.PhotoFile != null)
                {
                    int PhysId = p.PhysicianId;
                    string UploadPhoto = SaveFile.UploadDoc(pm.PhotoFile, PhysId);
                    p.Photo = UploadPhoto;
                    _context.Update(p);
                    _context.SaveChanges();
                }

                //PhysicianRegion Table
                List<int> priceList = pm.RegionsId.Split(',').Select(int.Parse).ToList();
                foreach (var item in priceList)
                {
                    PhysicianRegion pr = new();
                    pr.PhysicianId = p.PhysicianId;
                    pr.RegionId = item;
                    _context.Add(pr);
                    _context.SaveChanges();
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region DeleteAccount
        public bool DeleteAccount(int? id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }
                else
                {
                    var p = _context.Physicians.Where(W => W.PhysicianId == id).FirstOrDefault();
                    if (p != null)
                    {
                        p.IsDeleted = new BitArray(1);
                        p.IsDeleted[0] = true;
                        _context.Physicians.Update(p);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}