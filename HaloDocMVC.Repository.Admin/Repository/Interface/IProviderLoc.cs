using HaloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IProviderLoc
    {
        public List<PhysicianLocation> FindPhysicianLocation();
    }
}
