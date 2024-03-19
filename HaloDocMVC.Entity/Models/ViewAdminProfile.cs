using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class ViewAdminProfile
    {
        public int? AdminId { get; set; }

        public string? AspNetUserId { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public short? Status { get; set; }

        public int? RoleId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public string? AltMobile { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public int? RegionId { get; set; }

        public string? RegionsId { get; set; }

        public List<Regions>? RegionIds { get; set; }

        public string? ZipCode { get; set; }

        public string? CreatedBy { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? Ip { get; set; }

        public class Regions
        {
            public int? RegionId { get; set; }

            public string? RegionName { get; set; }
        }
    }
}
