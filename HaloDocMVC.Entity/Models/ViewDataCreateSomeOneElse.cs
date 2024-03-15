using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataCreateSomeOneElse
    {
        [Required(ErrorMessage = "Symptoms is Required!")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required!")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Street is Required!")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is Required!")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is Required!")]
        public string? State { get; set; }

        [Required(ErrorMessage = "ZipCode is Required!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string? ZipCode { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? UploadImage { get; set; }

        public string? RelationName { get; set; }
    }
}
