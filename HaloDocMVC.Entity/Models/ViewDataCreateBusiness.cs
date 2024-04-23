using System.ComponentModel.DataAnnotations;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataCreateBusiness
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "First Name is Required!")]
        public string? bFirstName { get; set; }

        public string Property { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is Required!")]
        public string pFirstName { get; set; } = string.Empty;

        public string? bLastName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? pLastName { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? bEmail { get; set; }

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
        public int State { get; set; }

        [Required(ErrorMessage = "ZipCode is Required!")]
        public string ZipCode { get; set; } = string.Empty;

        public string CaseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notes is Required!")]
        public string? Notes { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required!")]
        public DateTime DOB { get; set; }
    }
}
