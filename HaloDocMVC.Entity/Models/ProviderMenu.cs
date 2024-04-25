using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class ProviderMenu
    {
        public int? NotificationId { get; set; }

        public BitArray? Notification { get; set; }

        public string? Role { get; set; }

        public int? PhysicianId { get; set; }

        public string? AspNetUserId { get; set; }

        [Required(ErrorMessage = "User Name is Required!")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is Required!")]
        public string? PassWord { get; set; }
        
        public string? RegionsId { get; set; }

        [Required(ErrorMessage = "First Name is Required!")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is Required!")]
        public string? LastName { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Phone Number must contain digits!")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "State is Required!")]
        public int? State { get; set; }

        [Required(ErrorMessage = "Zipcode is Required!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZipCode must be of 6 digits!")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "ZipCode must contain digits!")]
        public string? ZipCode { get; set; }
        
        public string? MedicalLicense { get; set; }
        
        public string? Photo { get; set; }
        
        public IFormFile? PhotoFile { get; set; }
        
        public string? AdminNotes { get; set; }
        
        public bool IsAgreementDoc { get; set; }
        
        public bool IsBackgroundDoc { get; set; }
        
        public bool IsTrainingDoc { get; set; }
        
        public bool IsNonDisclosureDoc { get; set; }
        
        public bool IsLicenseDoc { get; set; }

        [Required(ErrorMessage = "Address1 is Required!")]
        public string? Address1 { get; set; }

        [Required(ErrorMessage = "Address2 is Required!")]
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is Required!")]
        public string? City { get; set; }
        
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Phone Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Phone Number must contain digits!")]
        public string? AltPhone { get; set; }
        
        public string? CreatedBy { get; set; } = null!;
        
        public DateTime? CreatedDate { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public short? Status { get; set; }

        [Required(ErrorMessage = "Business Name is Required!")]
        public string BusinessName { get; set; } = null!;

        [Required(ErrorMessage = "Business Website is Required!")]
        public string BusinessWebsite { get; set; } = null!;
        
        public BitArray? IsDeleted { get; set; }
        
        public int? RoleId { get; set; }
        
        public string? NpiNumber { get; set; }
        
        public string? Signature { get; set; }
        
        public IFormFile? SignatureFile { get; set; }
        
        public BitArray? IsCredentialDoc { get; set; }
        
        public BitArray? IsTokenGenerate { get; set; }

        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? SyncEmailAddress { get; set; }
        
        public IFormFile? AgreementDoc { get; set; }
        
        public IFormFile? NonDisclosureDoc { get; set; }
        
        public IFormFile? TrainingDoc { get; set; }
        
        public IFormFile? BackGroundDoc { get; set; }
        
        public IFormFile? LicenseDoc { get; set; }

        public int? OnCallStatus { get; set; } = 0;

        public int? Region { get; set; }

        public List<Regions>? RegionIds { get; set; }

        public List<ProviderMenu>? ProviderData { get; set; }
        
        public class Regions
        {
            public int? RegionId { get; set; }
        
            public string? RegionName { get; set; }
        }

        // Pagination
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool? IsAscending { get; set; } = true;

        public string? SortedColumn { get; set; } = "Name";
    }
}
