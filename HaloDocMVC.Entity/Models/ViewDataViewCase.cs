namespace HaloDocMVC.Entity.Models
{
    public class ViewDataViewCase
    {
        public int? UserId { get; set; }

        public int RequestTypeId { get; set; }

        public int Status { get; set; }

        public int RequestId { get; set; } 

        public string? ConfNo { get; set; }

        public string? Notes { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public DateTime DOB { get; set; }

        public string? Mobile { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Region { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
