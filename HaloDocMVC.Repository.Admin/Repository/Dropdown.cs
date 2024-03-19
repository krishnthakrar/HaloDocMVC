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

        public List<AllRegion> AllRegion()
        {
            return _context.Regions.Select(req => new AllRegion()
            {
                RegionId = req.RegionId,
                RegionName = req.Name
            }).ToList();
        }

        public List<CaseReason> CaseReason()
        {
            return _context.CaseTags.Select(req => new CaseReason()
            {
                CaseReasonId = req.CaseTagId,
                CaseReasonName = req.Name
            }).ToList();
        }

        public List<Physician> ProviderByRegion(int regionid)
        {
            var result = _context.Physicians
                            .Where(r => r.RegionId == regionid)
                            .OrderByDescending(x => x.CreatedDate)
                            .ToList();
            return result;
        }

        public List<HealthProfessionalTypes> HealthProfessionalType()
        {
            return _context.HealthProfessionalTypes.Select(req => new HealthProfessionalTypes()
            {
                HealthProfessionalId = req.HealthProfessionalId,
                ProfessionName = req.ProfessionName
            })
            .ToList();
        }

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

        public List<UserRole> UserRole()
        {
            return _context.AspNetRoles.Select(req => new UserRole()
            {
                RoleId = req.Id,
                RoleName = req.Name
            }).ToList();
        }
    }
}
