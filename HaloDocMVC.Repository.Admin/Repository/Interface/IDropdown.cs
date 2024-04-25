using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IDropdown
    {
        public List<AllRegion> AllRegion();

        public List<CaseReason> CaseReason();

        public List<Physician> ProviderByRegion(int regionid);

        public List<AllRegion> PhysiciansByRegion(int id);

        public List<HealthProfessionalTypes> HealthProfessionalType();

        public List<HealthProfessionals> ProfessionalByType(int? HealthProfessionalID);

        public List<UserRole> UserRole();

        public List<Role> PhysRole();

        public List<AspNetRole> AccType();

        public List<Menu> AccessByType(int AccountType);
    }
}
