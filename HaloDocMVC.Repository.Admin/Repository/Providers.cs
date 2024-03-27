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
    }
}
