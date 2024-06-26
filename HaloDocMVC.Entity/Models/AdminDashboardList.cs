﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class AdminDashboardList
    {
        public string? PatientName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Requestor { get; set; }
        
        public DateTime RequestedDate { get; set; }
        
        public string? PatientPhoneNumber { get; set; }
       
        public string? Email { get; set; }
        
        public string? RequestorPhoneNumber { get; set; }
       
        public int? RequestId { get; set; }
       
        public int? RequestTypeId { get; set; }
       
        public string? Address { get; set; }
        
        public string? Notes { get; set; }
        
        public int? ProviderId { get; set; }
        
        public string? ProviderName { get; set; }
       
        public string? Region { get; set; }
        
        public string? ConfirmationNumber { get; set; }

        public short Status { get; set; }

        public DateTime ConcludedDate { get; set; }

        public int? RequestClientId { get; set; }

        public int? RegionId { get; set; }

        public bool? IsFinalized { get; set; }
    }

    public class PaginatedViewModel
    {
        public List<AdminDashboardList>? adl { get; set; }
       
        public int CurrentPage { get; set; } = 1;
       
        public int TotalPages { get; set; } = 1;
       
        public int PageSize { get; set; } = 5;
      
        public string? SearchInput { get; set; }
       
        public int? RegionId { get; set; }
        
        public int? RequestType { get; set; }

        public bool? IsAscending { get; set; } = true;

        public string? SortedColumn { get; set; } = "PatientName";

        public string? SortedColumnPRecords { get; set; } = "PatientName";

        public int NewRequest { get; set; }
       
        public int PendingRequest { get; set; }
        
        public int ActiveRequest { get; set; }
        
        public int ConcludeRequest { get; set; }
        
        public int ToCloseRequest { get; set; }
        
        public int UnpaidRequest { get; set; }

        public int? UserId { get; set; }
    }
}
