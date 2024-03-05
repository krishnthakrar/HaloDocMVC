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

        Task<List<CaseReason>> CaseReason();

        public List<Physician> ProviderByRegion(int regionid);
    }
}
