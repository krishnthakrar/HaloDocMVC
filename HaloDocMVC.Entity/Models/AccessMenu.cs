using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class AccessMenu
    {
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public int AccountType { get; set;}

        public string? AccessId { get; set;}
    }
}
