using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class PartnersData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Profession is Required!")]
        public string? Profession { get; set; }

        [Required(ErrorMessage = "Business Name is Required!")]
        public string? Business { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? Email { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Fax Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Fax Number must contain digits!")]
        public string? FaxNumber { get; set; }

        [Required(ErrorMessage = "Phone Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Phone Number must contain digits!")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Business Contact is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Business Contact must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Business Contact must contain digits!")]
        public string? BusinessNumber { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? SearchData { get; set;}

        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string? ZipCode { get; set; }

        public List<PartnersData>? PD { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool? IsAscending { get; set; } = true;

        public string? SortedColumn { get; set; } = "Business";
    }
}
