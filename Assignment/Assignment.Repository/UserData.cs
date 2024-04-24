using Assignment.Entity.DataContext;
using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using Assignment.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment.Repository
{
    public class UserData : IUserData
    {
        private readonly UMSContext _context;

        public UserData(UMSContext context)
        {
            _context = context;
        }

        #region IndexData
        public PaginatedViewModel IndexData(PaginatedViewModel dataModel)
        {
            List<UserDataModel> result = (from user in _context.Users
                                          where (dataModel.SearchInput == null ||
                                                         user.FirstName.Contains(dataModel.SearchInput) || user.LastName.Contains(dataModel.SearchInput) ||
                                                         user.Email.Contains(dataModel.SearchInput) || user.PhoneNo.Contains(dataModel.SearchInput) ||
                                                         user.Gender.Contains(dataModel.SearchInput) || user.City.Contains(dataModel.SearchInput) ||
                                                         user.Country.Contains(dataModel.SearchInput))
                                          orderby user.Id
                                          select new UserDataModel
                                          {
                                              Id = user.Id,
                                              FirstName = user.FirstName,
                                              LastName = user.LastName,
                                              CityId = user.CityId,
                                              Age = user.Age.ToString(),
                                              Email = user.Email,
                                              PhoneNo = user.PhoneNo,
                                              Gender = user.Gender,
                                              City = user.City,
                                              Country = user.Country,
                                          }).ToList();
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)dataModel.PageSize);
            List<UserDataModel> list1 = result.Skip((dataModel.CurrentPage - 1) * dataModel.PageSize).Take(dataModel.PageSize).ToList();
            PaginatedViewModel paginatedViewModel = new()
            {
                UserData = list1,
                CurrentPage = dataModel.CurrentPage,
                TotalPages = totalPages,
                PageSize = dataModel.PageSize,
                SearchInput = dataModel.SearchInput
            };
            return paginatedViewModel;
        }
        #endregion

        #region AllCity
        public List<City> AllCity()
        {
            return _context.Cities.Select(req => new City()
            {
                Id = req.Id,
                Name = req.Name
            }).ToList();
        }
        #endregion

        #region AddUser
        public void AddUser(AddUserModel user)
        {
            var city = _context.Cities.FirstOrDefault(x => x.Id == user.CityId);
            User U = new();
            U.FirstName = user.FirstName;
            U.LastName = user.LastName;
            U.Email = user.Email;
            U.CityId = user.CityId;
            U.City = city.Name;
            U.Age = DateTime.Now.Year - user.DOB.Year;
            U.PhoneNo = user.PhoneNo;
            U.Gender = user.Gender;
            U.Country = user.Country;
            _context.Users.Add(U);
            _context.SaveChanges();
        }
        #endregion

        #region GetUserData
        public AddUserModel GetData(int id)
        {
            AddUserModel userData = (from user in _context.Users
                                 where (user.Id == id)
                                 select new AddUserModel
                                 {
                                     Id = user.Id,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     CityId = user.CityId,
                                     Age = user.Age.ToString(),
                                     Email = user.Email,
                                     PhoneNo = user.PhoneNo,
                                     Gender = user.Gender,
                                     Country = user.Country,
                                 }).First();
            return userData;
        }
        #endregion

        #region EditUser
        public void EditUser(AddUserModel user)
        {
            var city = _context.Cities.FirstOrDefault(x => x.Id == user.CityId);
            User U = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            U.FirstName = user.FirstName;
            U.LastName = user.LastName;
            U.Email = user.Email;
            U.CityId = user.CityId;
            U.City = city.Name;
            U.Age = DateTime.Now.Year - user.DOB.Year;
            U.PhoneNo = user.PhoneNo;
            U.Gender = user.Gender;
            U.Country = user.Country;
            _context.Users.Update(U);
            _context.SaveChanges();
        }
        #endregion

        #region DeleteUser
        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
