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
        public ProviderMenu PhysicianAll(ProviderMenu pm);

        public bool ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);

        public ProviderMenu PhysicianByRegion(ProviderMenu pm, int? region);

        public ProviderMenu GetProfileDetails(int UserId);

        public bool EditPassword(string password, ProviderMenu pm);

        public bool EditPhysInfo(ProviderMenu pm);

        public bool BillingInfoEdit(ProviderMenu pm);

        public bool ProviderInfoEdit(ProviderMenu pm);

        public bool ProviderEditSubmit(ProviderMenu pm);

        public bool CreateProvider(ProviderMenu pm, string? id);

        public bool DeleteAccount(int? id);
    }
}
