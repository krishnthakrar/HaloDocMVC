using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class AccessMenu
    {
        [Required(ErrorMessage = "Account Type is Required!")]
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public int AccountType { get; set;}

        public string? AccessId { get; set;}

        public List<RoleMenus>? RoleMenuList { get; set; }

        public class RoleMenus
        {
            public int? RoleMenuId { get; set; }

            public int? RoleId { get; set; }

            public int? MenuId { get; set; }
        }
    }
}
