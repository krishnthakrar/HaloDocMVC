using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Interface
{
    public interface IUserData
    {
        public PaginatedViewModel IndexData(PaginatedViewModel dataModel);

        public List<City> AllCity();

        public void AddUser(AddUserModel user);

        public void DeleteUser(int id);

        public AddUserModel GetData(int id);

        public void EditUser(AddUserModel user);
    }
}
