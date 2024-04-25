using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataViewDocuments
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ConfirmationNumber { get; set; }
        public string? Email { get; set; }
        public int RequestId { get; set; }
        public class Documents
        {
            public string? Uploader { get; set; }
            public int? Status { get; set; }
            public string? FileName { get; set; }
            public DateTime CreatedDate { get; set; }
            public int? RequestWiseFilesId { get; set; }
            public string? IsDeleted { get; set; }
        }
        public List<Documents>? DocumentsList { get; set; } = null;

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool? IsAscending { get; set; } = true;

        public string? SortedColumn { get; set; } = "CreatedDate";
    }
}
