using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public List<ProviderMenu> PhysicianAll()
        {
            List<ProviderMenu> data = (from r in _context.Physicians
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
                                           FirstName = r.FirstName,
                                           LastName = r.LastName,
                                           Notification = nof.IsNotificationStopped,
                                           Role = roles.Name,
                                           Status = r.Status,
                                           Email = r.Email,
                                           IsNonDisclosureDoc = r.IsNonDisclosureDoc
                                       }).ToList();
            return data;
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
        public List<ProviderMenu> PhysicianByRegion(int? region)
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
                                            FirstName = r.FirstName,
                                            LastName = r.LastName,
                                            Notification = nof.IsNotificationStopped,
                                            Role = roles.Name,
                                            Status = r.Status,
                                            Email = r.Email,
                                            IsNonDisclosureDoc = r.IsNonDisclosureDoc
                                        }).ToList();
            return data;
        }
        #endregion

        #region GetProfile
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
                                       AdminNotes = r.AdminNotes
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
        public bool EditPassword(string password, int physId, ProviderMenu pm)
        {
            var req = _context.Physicians.Where(W => W.PhysicianId == physId).FirstOrDefault();
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
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}