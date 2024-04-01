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
        public List<AccessMenu> AccessIndex()
        {
            List<AccessMenu> data = (from r in _context.Roles
                                     where r.IsDeleted == new BitArray(1)
                                     select new AccessMenu
                                     {
                                        RoleName = r.Name,
                                        RoleId = r.RoleId,
                                        AccountType = r.AccountType
                                     }).ToList();
            return data;
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
    }
}
