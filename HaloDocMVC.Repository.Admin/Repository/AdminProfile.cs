using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaloDocMVC.Entity.Models.ViewAdminProfile;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class AdminProfile : IAdminProfile
    {
        private readonly HaloDocContext _context;

        public AdminProfile(HaloDocContext context)
        {
            _context = context;
        }

        #region GetProfile
        public ViewAdminProfile GetProfileDetails(int UserId)
        {
            ViewAdminProfile? v = (from r in _context.Admins
                                         join Aspnetuser in _context.AspNetUsers
                                         on r.AspNetUserId equals Aspnetuser.Id into aspGroup
                                         from asp in aspGroup.DefaultIfEmpty()
                                         where r.AdminId == UserId
                                         select new ViewAdminProfile
                                         {
                                             RoleId = r.RoleId,
                                             AdminId = r.AdminId,
                                             UserName = asp.UserName,
                                             Address1 = r.Address1,
                                             Address2 = r.Address2,
                                             AltMobile = r.AltPhone,
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
                                             ZipCode = r.Zip
                                         }).FirstOrDefault();
            List<Regions> regions = new();
            regions = _context.AdminRegions
                  .Where(r => r.AdminId == UserId)
                  .Select(req => new Regions()
                  {
                      RegionId = req.RegionId
                  }).ToList();
            v.RegionIds = regions;
            return v;
        }
        #endregion

        #region EditPassword
        public bool EditPassword(string password, int adminId)
        {
            var req = _context.Admins.Where(W => W.AdminId == adminId).FirstOrDefault();
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

        #region EditAdministratorInfo
        public bool EditAdministratorInfo(ViewAdminProfile _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = _viewAdminProfile.Email;
                        DataForChange.FirstName = _viewAdminProfile.FirstName;
                        DataForChange.LastName = _viewAdminProfile.LastName;
                        DataForChange.Mobile = _viewAdminProfile.Mobile;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId).Select(req => req.RegionId).ToList();
                        List<int> priceList = _viewAdminProfile.RegionsId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                AdminRegion ar = new()
                                {
                                    RegionId = item,
                                    AdminId = (int)_viewAdminProfile.AdminId
                                };
                                _context.AdminRegions.Update(ar);
                                _context.SaveChanges();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                AdminRegion ar = _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId && r.RegionId == item).First();
                                _context.AdminRegions.Remove(ar);
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

        #region BillingInfoEdit
        public bool BillingInfoEdit(ViewAdminProfile _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = _viewAdminProfile.Address1;
                        DataForChange.Address2 = _viewAdminProfile.Address2;
                        DataForChange.City = _viewAdminProfile.City;
                        DataForChange.Mobile = _viewAdminProfile.Mobile;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
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
