using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface ILogin
    {
        Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser);

        public bool SendResetLink(String Email);

        public bool SendResetLinkPatient(String Email);
    }
}
