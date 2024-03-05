using static HaloDocMVC.Entity.Models.Constant;

namespace HaloDocMVC.Entity.Models
{
    public class DashboardList
    {
        public DateTime createdDate { get; set; }
        public AdminDashStatus Status { get; set; }
        public int RequestId { get; set; }
        public int Fcount { get; set; }
    }
}
