using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IRequestRepository
    {
        public CountStatusWiseRequestModel IndexData();

        public List<AdminDashboardList> NewRequestData();

        public List<AdminDashboardList> GetRequests(string Status);
    }
}
