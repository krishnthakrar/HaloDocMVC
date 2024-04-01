using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Dropdown : IDropdown
    {
        private readonly HaloDocContext _context;

        public Dropdown(HaloDocContext context)
        {
            _context = context;
        }

        #region RegionModel
        public List<AllRegion> AllRegion()
        {
            return _context.Regions.Select(req => new AllRegion()
            {
                RegionId = req.RegionId,
                RegionName = req.Name
            }).ToList();
        }
        #endregion

        #region CaseReasonModel
        public List<CaseReason> CaseReason()
        {
            return _context.CaseTags.Select(req => new CaseReason()
            {
                CaseReasonId = req.CaseTagId,
                CaseReasonName = req.Name
            }).ToList();
        }
        #endregion

        #region ProviderByRegion
        public List<Physician> ProviderByRegion(int regionid)
        {
            var result = _context.Physicians
                            .Where(r => r.RegionId == regionid)
                            .OrderByDescending(x => x.CreatedDate)
                            .ToList();
            return result;
        }
        #endregion

        #region HealthProfessionalType
        public List<HealthProfessionalTypes> HealthProfessionalType()
        {
            return _context.HealthProfessionalTypes.Select(req => new HealthProfessionalTypes()
            {
                HealthProfessionalId = req.HealthProfessionalId,
                ProfessionName = req.ProfessionName
            })
            .ToList();
        }
        #endregion

        #region ProfessionalByType
        public List<HealthProfessionals> ProfessionalByType(int? HealthProfessionalID)
        {
            var result = _context.HealthProfessionals
                        .Where(r => r.Profession == HealthProfessionalID)
                        .Select(req => new HealthProfessionals()
                        {
                            VendorId = req.VendorId,
                            VendorName = req.VendorName
                        }).ToList();
            return result;
        }
        #endregion

        #region UserRole
        public List<UserRole> UserRole()
        {
            return _context.AspNetRoles.Select(req => new UserRole()
            {
                RoleId = req.Id,
                RoleName = req.Name
            }).ToList();
        }
        #endregion

        #region PhysRole
        public List<Role> PhysRole()
        {
            return _context.Roles.Select(req => new Role()
            {
                RoleId = req.RoleId,
                Name = req.Name
            }).ToList();
        }
        #endregion

        #region AccType
        public List<AspNetRole> AccType()
        {
            return _context.AspNetRoles.Select(req => new AspNetRole()
            {
                Id = req.Id,
                Name = req.Name
            }).ToList();
        }
        #endregion

        #region AccessRole
        public List<Menu> AccessRole()
        {
            return _context.Menus.Select(req => new Menu()
            {
                MenuId = req.MenuId,
                Name = req.Name,
                AccountType = req.AccountType,
            }).ToList();
        }
        #endregion

        #region HealthProfessionalType
        public List<Menu> AccessByType(int AccountType)
        {
            return _context.Menus.Where(r => r.AccountType == (short)AccountType).Select(req => new Menu()
            {
                MenuId = req.MenuId,
                Name = req.Name
            })
            .ToList();
        }
        #endregion
    }
}
