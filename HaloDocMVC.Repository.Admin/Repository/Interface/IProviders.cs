using HaloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IProviders
    {
        public List<ProviderMenu> PhysicianAll();

        public bool ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);

        public List<ProviderMenu> PhysicianByRegion(int? region);

        public ProviderMenu GetProfileDetails(int UserId);

        public bool EditPassword(string password, ProviderMenu pm);

        public bool EditPhysInfo(ProviderMenu pm);

        public bool BillingInfoEdit(ProviderMenu pm);

        public bool ProviderInfoEdit(ProviderMenu pm, IFormFile? file, IFormFile? file1);
    }
}
