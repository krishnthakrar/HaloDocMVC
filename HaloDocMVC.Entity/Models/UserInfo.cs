using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Role { get; set; }

        public string AspNetUserId { get; set; }
    }
}
