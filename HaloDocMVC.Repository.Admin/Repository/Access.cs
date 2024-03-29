using HaloDocMVC.Entity.DataContext;
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
    }
}
