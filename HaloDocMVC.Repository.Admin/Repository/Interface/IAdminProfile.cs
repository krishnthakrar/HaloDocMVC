using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IAdminProfile
    {
        public ViewAdminProfile GetProfileDetails(int UserId);
        public bool EditPassword(string password, int adminId);
        public bool EditAdministratorInfo(ViewAdminProfile _viewAdminProfile);
        public bool BillingInfoEdit(ViewAdminProfile _viewAdminProfile);
    }
}
