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

                    RoleMenu RM = new();
                    List<int> priceList = am.AccessId.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        RM.RoleId = R.RoleId;
                        RM.MenuId = item;
                        _context.RoleMenus.Update(RM);
                        _context.SaveChanges();
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
    }
}
