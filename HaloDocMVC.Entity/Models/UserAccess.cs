using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class UserAccess
    {
        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? Email { get; set; }

        public short? AccountType { get; set; }

        public short? Status { get; set; }

        public int? OpenRequest { get; set; }

        public string? Mobile { get; set; }

        public bool IsAdmin { get; set; }

        public int? PhysicianId { get; set; }
    }
}
