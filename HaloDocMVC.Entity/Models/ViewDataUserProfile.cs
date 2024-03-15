using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataUserProfile
    {
        public int? UserId { get; set; }

        public string? AspNetUserId { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? Mobile { get; set; }

        public BitArray? IsMobile { get; set; }

        [Required(ErrorMessage = "Street is Required!")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is Required!")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is Required!")]
        public string? State { get; set; }

        public int? RegionId { get; set; }

        [Required(ErrorMessage = "ZipCode is Required!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string? ZipCode { get; set; }

        public string? StrMonth { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required!")]
        public DateTime DOB { get; set; }

        public int? IntYear { get; set; }

        public int? IntDate { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public short? Status { get; set; }

        public string? Ip { get; set; }
    }
}