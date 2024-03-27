using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class ProviderLoc : IProviderLoc
    {
        private readonly HaloDocContext _context;

        public ProviderLoc(HaloDocContext context)
        {
            _context = context;
        }

        #region Find_Location_Physician
        public List<PhysicianLocation> FindPhysicianLocation()
        {
            List<PhysicianLocation> pl = _context.PhysicianLocations
                                    .OrderByDescending(x => x.PhysicianName)
                        .Select(r => new PhysicianLocation
                        {
                            LocationId = r.LocationId,
                            Longitude = r.Longitude,
                            Latitude = r.Latitude,
                            PhysicianName = r.PhysicianName

                        }).ToList();
            return pl;
        }
        #endregion
    }
}
