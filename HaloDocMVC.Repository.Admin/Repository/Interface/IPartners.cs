﻿using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IPartners
    {
        public List<PartnersData> GetPartnersByProfession(string searchValue, int Profession);

        public bool AddBusiness(PartnersData pd);

        public PartnersData EditBusiness(int id);

        public bool EditBusinessSubmit(PartnersData pd);

        public void DeleteBusiness(int BusinessId);
    }
}