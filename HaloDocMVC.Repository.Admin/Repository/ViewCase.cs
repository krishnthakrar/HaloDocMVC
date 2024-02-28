using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class ViewCase : IViewCase
    {
        private readonly HaloDocContext _context;
        public ViewCase(HaloDocContext context)
        {
            _context = context;
        }
        public ViewDataViewCase NewRequestData(int? RId, int? RTId)
        {
            ViewDataViewCase caseList = _context.RequestClients
                                        .Where(r => r.Request.RequestId == RId)
                                        .Select(req => new ViewDataViewCase()
                                        {
                                            RequestId = (int)RId,
                                            RequestTypeId = (int)RTId,
                                            ConfNo = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                                            Notes = req.Notes,
                                            FirstName = req.FirstName,
                                            LastName = req.LastName,
                                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                                            Mobile = req.PhoneNumber,
                                            Email = req.Email,
                                            Address = req.Address
                                        }).FirstOrDefault();

            return caseList;
        }
    }
}
