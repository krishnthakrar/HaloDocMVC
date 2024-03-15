using System.ComponentModel.DataAnnotations;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataCreateConcierge
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "First Name is Required!")]
        public string? cFirstName { get; set; }

        public string? pName { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string pFirstName { get; set; } = string.Empty;

        public string? cLastName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? pLastName { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? cEmail { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string pEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Street is Required!")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is Required!")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is Required!")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "ZipCode is Required!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notes is Required!")]
        public string? Notes { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required!")]
        public DateTime DOB { get; set; }
    }
}
