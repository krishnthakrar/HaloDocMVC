using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public string? UserName { get; set; }
        
        public string? PassWord { get; set; }
        
        public string? RegionsId { get; set; }
        
        public string FirstName { get; set; } = null!;
        
        public string? LastName { get; set; }
        
        public string Email { get; set; } = null!;
        
        public string? Mobile { get; set; }
        
        public string? State { get; set; }
        
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
        
        public string? Address1 { get; set; }
        
        public string? Address2 { get; set; }
        
        public string? City { get; set; }
        
        public int? RegionId { get; set; }
        
        public string? AltPhone { get; set; }
        
        public string? CreatedBy { get; set; } = null!;
        
        public DateTime? CreatedDate { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public short? Status { get; set; }
        
        public string BusinessName { get; set; } = null!;
        
        public string BusinessWebsite { get; set; } = null!;
        
        public BitArray? IsDeleted { get; set; }
        
        public int? RoleId { get; set; }
        
        public string? NpiNumber { get; set; }
        
        public string? Signature { get; set; }
        
        public IFormFile? SignatureFile { get; set; }
        
        public BitArray? IsCredentialDoc { get; set; }
        
        public BitArray? IsTokenGenerate { get; set; }
        
        public string? SyncEmailAddress { get; set; }
        
        public IFormFile? AgreementDoc { get; set; }
        
        public IFormFile? NonDisclosureDoc { get; set; }
        
        public IFormFile? TrainingDoc { get; set; }
        
        public IFormFile? BackGroundDoc { get; set; }
        
        public IFormFile? LicenseDoc { get; set; }

        public int? OnCallStatus { get; set; } = 0;

        public List<Regions>? RegionIds { get; set; }
        
        public class Regions
        {
            public int? RegionId { get; set; }
        
            public string? RegionName { get; set; }
        }
    }
}
