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
                          where (Hp.IsDeleted == new BitArray(1))
                             && (searchValue == null || Hp.VendorName.Contains(searchValue))
                             && (Profession == 0 || Hp.Profession == Profession)
                          orderby Hp.VendorId
                          select new PartnersData
                          {
                              Id = Hp.VendorId,
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

        #region EditBusiness
        public PartnersData EditBusiness(int id)
        {
            PartnersData pd = (from hp in _context.HealthProfessionals
                               where hp.VendorId == id
                               select new PartnersData
                               {
                                   Id = hp.VendorId,
                                   Profession = hp.Profession.ToString(),
                                   Business = hp.VendorName,
                                   Email = hp.Email,
                                   FaxNumber = hp.FaxNumber,
                                   PhoneNumber = hp.PhoneNumber,
                                   BusinessNumber = hp.BusinessContact
                               }).FirstOrDefault();
            return pd;
        }

        public bool EditBusinessSubmit(PartnersData pd)
        {
            HealthProfessional HP = _context.HealthProfessionals.FirstOrDefault(m => m.VendorId == pd.Id);
            if (HP != null)
            {
                HP.VendorName = pd.Business;
                HP.Profession = Int32.Parse(pd.Profession);
                HP.FaxNumber = pd.FaxNumber;
                HP.City = pd.City != null ? pd.City : HP.City;
                HP.State = pd.State != null ? pd.State : HP.State;
                HP.Zip = pd.ZipCode != null ? pd.ZipCode : HP.Zip;
                HP.Address = pd.Street + ", " + HP.City + ", " + HP.State + ", " + HP.Zip;
                HP.ModifiedDate = DateTime.Now;
                HP.PhoneNumber = pd.PhoneNumber;
                HP.Email = pd.Email;
                HP.BusinessContact = pd.BusinessNumber;
                _context.Update(HP);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region DeleteBusiness
        public void DeleteBusiness(int BusinessId)
        {
            HealthProfessional HP = _context.HealthProfessionals.FirstOrDefault(m => m.VendorId == BusinessId);
            if (HP != null)
            {
                HP.IsDeleted = new BitArray(1);
                HP.IsDeleted[0] = true;
                _context.Update(HP);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
