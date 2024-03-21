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
        PaginatedViewModel IndexData();

        PaginatedViewModel GetRequests(string Status, PaginatedViewModel data);

        List<AdminDashboardList> Export(string status);
    }
}
