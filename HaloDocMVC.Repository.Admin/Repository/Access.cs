using DocumentFormat.OpenXml.Spreadsheet;
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
using static HaloDocMVC.Entity.Models.AccessMenu;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Access : IAccess
    {
        private readonly HaloDocContext _context;
        public Access(HaloDocContext context)
        {
            _context = context;
        }

        #region Index
        public AccessMenu AccessIndex(AccessMenu am)
        {
            List<AccessMenu> data = (from r in _context.Roles
                                     where r.IsDeleted == new BitArray(1)
                                     select new AccessMenu
                                     {
                                        RoleName = r.Name,
                                        RoleId = r.RoleId,
                                        AccountType = r.AccountType
                                     }).ToList();
            if (am.IsAscending == true)
            {
                data = am.SortedColumn switch
                {
                    "PhysicianName" => data.OrderBy(x => x.RoleName).ToList(),
                    _ => data.OrderBy(x => x.RoleName).ToList()
                };
            }
            else
            {
                data = am.SortedColumn switch
                {
                    "PhysicianName" => data.OrderByDescending(x => x.RoleName).ToList(),
                    _ => data.OrderByDescending(x => x.RoleName).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)am.PageSize);
            List<AccessMenu> list = data.Skip((am.CurrentPage - 1) * am.PageSize).Take(am.PageSize).ToList();

            AccessMenu model = new()
            {
                AM = list,
                CurrentPage = am.CurrentPage,
                TotalPages = totalPages,
                PageSize = am.PageSize,
                IsAscending = am.IsAscending,
                SortedColumn = am.SortedColumn
            };

            return model;
        }
        #endregion

        #region CreateAccessPost
        public bool CreateAccessPost(AccessMenu am, string id)
        {
            try
            {
                if (am == null)
                {
                    return false;
                }
                else
                {
                    Role R = new();
                    R.Name = am.RoleName;
                    R.AccountType = (short)am.RoleId;
                    R.CreatedBy = id;
                    R.CreatedDate = DateTime.Now;
                    R.IsDeleted = new BitArray(1);
                    R.IsDeleted[0] = false;
                    _context.Roles.Add(R);
                    _context.SaveChanges();

                    if (am.AccessId != null)
                    {
                        List<int>? priceList = am.AccessId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            RoleMenu RM = new();
                            RM.RoleId = R.RoleId;
                            RM.MenuId = item;
                            _context.RoleMenus.Add(RM);
                            _context.SaveChanges();
                        }
                        return true;
                    }
                    else 
                    {
                        return true; 
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DeleteAccess
        public bool DeleteAccess(int? id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }
                else
                {
                    var r = _context.Roles.Where(W => W.RoleId == id).FirstOrDefault();
                    if (r != null)
                    {
                        r.IsDeleted = new BitArray(1);
                        r.IsDeleted[0] = true;
                        _context.Roles.Update(r);
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

        #region EditAccess
        public AccessMenu EditAccess(int? id)
        {
            AccessMenu? am = (from r in _context.Roles
                               join rm in _context.RoleMenus
                               on r.RoleId equals rm.RoleId into roleGrp
                               from Role in roleGrp.DefaultIfEmpty()
                               where r.RoleId == id
                               select new AccessMenu
                               {
                                   RoleId = r.RoleId,
                                   RoleName = r.Name,
                                   AccountType = r.AccountType
                               }).FirstOrDefault();
            List<RoleMenus> roleMenu = new();
            roleMenu = _context.RoleMenus
                  .Where(r => r.RoleId == id)
                  .Select(req => new RoleMenus()
                  {
                      RoleMenuId = req.RoleMenuId,
                      RoleId = req.RoleId,
                      MenuId = req.MenuId,
                  }).ToList();
            am.RoleMenuList = roleMenu;
            return am;
        }
        #endregion

        #region EditAccessPost
        public bool EditAccessPost(AccessMenu am)
        {
            try
            {
                if (am == null)
                {
                    return false;
                }
                else
                {
                    var RoleChange = _context.Roles.Where(W => W.RoleId == am.RoleId).FirstOrDefault();
                    if (RoleChange != null)
                    {
                        RoleChange.Name = am.RoleName;
                        _context.Roles.Update(RoleChange);
                        _context.SaveChanges();

                        List<int> accessRole = _context.RoleMenus.Where(r => r.RoleId == am.RoleId).Select(req => req.MenuId).ToList();
                        List<int> priceList = am.AccessId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (accessRole.Contains(item))
                            {
                                accessRole.Remove(item);
                            }
                            else
                            {
                                RoleMenu rm = new()
                                {
                                    RoleId = am.RoleId,
                                    MenuId = item,
                                };
                                _context.RoleMenus.Update(rm);
                                _context.SaveChanges();
                                accessRole.Remove(item);
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

        #region GetAllUserDetails
        public UserAccess GetAllUserDetails(int? User, UserAccess ua)
        {
            var userDetails = (from user in _context.AspNetUsers
                                 join admin in _context.Admins on user.Id equals admin.AspNetUserId into adminGroup
                                 from admin in adminGroup.DefaultIfEmpty()
                                 join physician in _context.Physicians on user.Id equals physician.AspNetUserId into physicianGroup
                                 from physician in physicianGroup.DefaultIfEmpty()
                                 where (admin != null || physician != null) &&
                                       (admin.IsDeleted == new BitArray(1) || physician.IsDeleted == new BitArray(1))
                                 select new UserAccess
                                 {
                                     UserName = user.UserName,
                                     FirstName = admin != null ? admin.FirstName : (physician != null ? physician.FirstName : null),
                                     IsAdmin = admin != null ? true : false,
                                     UserId = admin != null ? admin.AdminId : (physician != null ? physician.PhysicianId : null),
                                     AccountType = admin != null ? 1 : (physician != null ? 2 : null),
                                     Status = admin != null ? admin.Status : (physician != null ? physician.Status : null),
                                     Mobile = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                                     PhysicianId = physician.PhysicianId // Keep the Physician ID for counting requests
                                 }).ToList();

            // Prepare the final list with RequestCount calculated
            var result = userDetails.Select(u => new UserAccess
            {
                UserName = u.UserName,
                FirstName = u.FirstName,
                IsAdmin = u.IsAdmin,
                UserId = u.UserId,
                AccountType = u.AccountType,
                Status = u.Status,
                Mobile = u.Mobile,
                // Calculate RequestCount for each physician
                OpenRequest = u.PhysicianId.HasValue ? _context.Requests.Count(r => r.PhysicianId == u.PhysicianId) : 0
            }).ToList();

            // Further filtering based on User input
            if (User.HasValue)
            {
                switch (User.Value)
                {
                    case 1: // Admin data
                        result = result.Where(u => u.IsAdmin).ToList();
                        break;
                    case 2: // Provider data
                        result = result.Where(u => !u.IsAdmin).ToList();
                        break;
                    case 3:
                        result = new List<UserAccess>();
                        break;
                    default: 
                        break;
                }
            }

            if (ua.IsAscending == true)
            {
                result = ua.SortedColumn switch
                {
                    "AccountType" => result.OrderBy(x => x.AccountType).ToList(),
                    "FirstName" => result.OrderBy(x => x.FirstName).ToList(),
                    _ => result.OrderBy(x => x.FirstName).ToList()
                };
            }
            else
            {
                result = ua.SortedColumn switch
                {
                    "AccountType" => result.OrderByDescending(x => x.AccountType).ToList(),
                    "FirstName" => result.OrderByDescending(x => x.FirstName).ToList(),
                    _ => result.OrderByDescending(x => x.FirstName).ToList()
                };
            }
            int totalItemCount = result.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)ua.PageSize);
            List<UserAccess> list = result.Skip((ua.CurrentPage - 1) * ua.PageSize).Take(ua.PageSize).ToList();

            UserAccess model = new()
            {
                UA = list,
                CurrentPage = ua.CurrentPage,
                TotalPages = totalPages,
                PageSize = ua.PageSize,
                IsAscending = ua.IsAscending,
                SortedColumn = ua.SortedColumn
            };

            return model;
        }
        #endregion

        #region CreateAdmin
        public bool CreateAdmin(ViewAdminProfile vap, string? id)
        {
            if (vap == null)
            {
                return false;
            }
            else
            {
                AspNetUser A = new();
                AspNetUserRole AUR = new();
                Entity.DataModels.Admin AD = new();

                var isexist = _context.AspNetUsers.FirstOrDefault(w => w.Email == vap.Email);
                if (isexist == null)
                {
                    //AspNetUser Table
                    Guid g = Guid.NewGuid();
                    A.Id = g.ToString();
                    A.UserName = vap.UserName;
                    A.PasswordHash = vap.Password;
                    A.Email = vap.Email;
                    A.PhoneNumber = vap.Mobile;
                    A.CreatedDate = DateTime.Now;
                    _context.Add(A);
                    _context.SaveChanges();

                    AUR.UserId = A.Id;
                    AUR.RoleId = "1";
                    _context.Add(AUR);
                    _context.SaveChanges();

                    AD.AspNetUserId = A.Id;
                }
                else
                {
                    AD.AspNetUserId = isexist.Id;
                }
                AD.FirstName = vap.FirstName;
                AD.LastName = vap.LastName;
                AD.Email = vap.Email;
                AD.Mobile = vap.Mobile;
                AD.Address1 = vap.City + ", " + vap.State + ", " + vap.ZipCode;
                AD.Address2 = AD.Address1;
                AD.City = vap.City;
                AD.Zip = vap.ZipCode;
                AD.AltPhone = vap.AltMobile;
                AD.CreatedBy = id;
                AD.CreatedDate = DateTime.Now;
                AD.Status = 1;
                AD.IsDeleted = new BitArray(1);
                AD.IsDeleted[0] = false;
                _context.Add(AD);
                _context.SaveChanges();

                List<int> priceList = vap.RegionsId.Split(',').Select(int.Parse).ToList();
                foreach (var item in priceList)
                {
                    AdminRegion AR = new();
                    AR.AdminId = AD.AdminId;
                    AR.RegionId = item;
                    _context.Add(AR);
                    _context.SaveChanges();
                }
                return true;
            }
        }
        #endregion
    }
}
