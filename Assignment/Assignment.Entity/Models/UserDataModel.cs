using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Entity.Models
{
    public class UserDataModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set;}

        public string? Email { get; set; }

        public int? CityId { get; set; }

        public string? Age { get; set; }

        public string? PhoneNo { get; set; }

        public string? Gender { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }
    }

    public class PaginatedViewModel
    {
        public List<UserDataModel>? UserData { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchInput { get; set; }
    }
}
