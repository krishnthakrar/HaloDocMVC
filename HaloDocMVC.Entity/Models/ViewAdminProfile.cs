using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class ViewAdminProfile
    {
        public int? AdminId { get; set; }

        public string? AspNetUserId { get; set; }

        [Required(ErrorMessage = "User Name is Required!")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is Required!")]
        public string? Password { get; set; }

        public short? Status { get; set; }

        public int? RoleId { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? AltMobile { get; set; }

        [Required(ErrorMessage = "Address1 is Required!")]
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public int? RegionId { get; set; }

        public string? RegionsId { get; set; }

        public List<Regions>? RegionIds { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Phone Number must contain digits!")]
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
