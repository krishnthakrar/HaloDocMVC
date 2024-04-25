using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataCreateFriend
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "First Name is Required!")]
        public string? fFirstName { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string pFirstName { get; set; } = string.Empty;

        public string? fLastName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? pLastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "phone Number must contain digits!")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? fEmail { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string pEmail { get; set; } = string.Empty;

        public string? RelationName { get; set; }

        [Required(ErrorMessage = "Mobile Number is Required!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Street is Required!")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is Required!")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is Required!")]
        public int? State { get; set; }

        [Required(ErrorMessage = "ZipCode is Required!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required!")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Symptoms is Required!")]
        public string? Notes { get; set; }

        public IFormFile? UploadFile { get; set; }

        public string? UploadImage { get; set; }
    }
}
