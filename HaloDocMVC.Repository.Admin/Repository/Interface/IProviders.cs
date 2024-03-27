using HaloDocMVC.Entity.Models;
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
    }
}
