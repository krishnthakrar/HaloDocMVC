using static HaloDocMVC.Entity.Models.Constant;

namespace HaloDocMVC.Entity.Models
{
    public class DashboardList
    {
        public int UserId { get; set; }

        public DateTime createdDate { get; set; }

        public short Status { get; set; }

        public int RequestId { get; set; }

        public int Fcount { get; set; }

        public List<DashboardList>? patientdata { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int PageSize { get; set; } = 5;

        public bool? IsAscending { get; set; } = false;

        public string? SortedColumn { get; set; } = "createdDate";
    }
}
