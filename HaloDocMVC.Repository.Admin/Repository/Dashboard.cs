using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static HaloDocMVC.Entity.Models.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocMVC.Entity.Models;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Dashboard : IDashboard
    {
        private readonly HaloDocContext _context;
        public Dashboard(HaloDocContext context)
        {
            _context = context;
        }
        public List<DashboardList> DashboardList(int? id)
        {
            var items = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == id).Select(x => new DashboardList
            {
                createdDate = x.CreatedDate,
                Status = (AdminDashStatus)x.Status,
                RequestId = x.RequestId,
                Fcount = x.RequestWiseFiles.Count()

            }).ToList();
            return items;
        }
    }
}
