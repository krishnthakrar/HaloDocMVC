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
    public class Partners : IPartners
    {
        private readonly HaloDocContext _context;

        public Partners(HaloDocContext context)
        {
            _context = context;
        }

        #region PartnersIndex
        public List<PartnersData> GetPartnersByProfession(string searchValue, int Profession)
        {
            var result = (from Hp in _context.HealthProfessionals
                          join Hpt in _context.HealthProfessionalTypes
                          on Hp.Profession equals Hpt.HealthProfessionalId into AdminGroup
                          from asp in AdminGroup.DefaultIfEmpty()
                          where (searchValue == null || Hp.VendorName.Contains(searchValue))
                             && (Profession == 0 || Hp.Profession == Profession)
                          select new PartnersData
                          {
                              Profession = asp.ProfessionName,
                              Business = Hp.VendorName,
                              Email = Hp.Email,
                              FaxNumber = Hp.FaxNumber,
                              PhoneNumber = Hp.PhoneNumber,
                              BusinessNumber = Hp.BusinessContact
                          }).ToList();
            return result;
        }
        #endregion

        #region AddBusiness
        public bool AddBusiness(PartnersData pd)
        {
            try
            {
                HealthProfessional HP = new();
                var isexist = _context.HealthProfessionals.FirstOrDefault(x => x.Email == pd.Email);
                if (isexist == null)
                {
                    HP.VendorName = pd.Business;
                    HP.Profession = Int32.Parse(pd.Profession);
                    HP.FaxNumber = pd.FaxNumber;
                    HP.Address = pd.Street + ", " + pd.City + ", " + pd.State + ", " + pd.ZipCode;
                    HP.City = pd.City;
                    HP.State = pd.State;
                    HP.Zip = pd.ZipCode;
                    HP.RegionId = 2;
                    HP.CreatedDate = DateTime.Now;
                    HP.PhoneNumber = pd.PhoneNumber;
                    HP.IsDeleted = new BitArray(1);
                    HP.IsDeleted[0] = false;
                    HP.Email = pd.Email;
                    HP.BusinessContact = pd.BusinessNumber;
                    _context.Add(HP);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
