using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Buttons : IButtons
    {
        private readonly HaloDocContext _context;

        public Buttons(HaloDocContext context)
        {
            _context = context;
        }

        public void CreateRequest(ViewDataCreatePatient vdcp)
        {
            AspNetUser A = new();
            User U = new();
            Request R = new();
            RequestClient RC = new();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcp.Email);
            if (isexist == null)
            {
                //AspNetUser Table
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = vdcp.UserName;
                A.PasswordHash = vdcp.PassWord;
                A.Email = vdcp.Email;
                A.PhoneNumber = vdcp.Mobile;
                A.CreatedDate = DateTime.Now;
                _context.Add(A);
                _context.SaveChanges();
                //User Table
                U.AspNetUserId = A.Id;
                U.FirstName = vdcp.FirstName;
                U.LastName = vdcp.LastName;
                U.Email = vdcp.Email;
                U.Mobile = vdcp.Mobile;
                U.Street = vdcp.Street;
                U.City = vdcp.City;
                U.State = vdcp.State;
                U.ZipCode = vdcp.ZipCode;
                U.StrMonth = (vdcp.DOB.Month).ToString();
                U.IntDate = vdcp.DOB.Day;
                U.IntYear = vdcp.DOB.Year;
                U.CreatedBy = A.Id;
                U.CreatedDate = DateTime.Now;
                _context.Add(U);
                _context.SaveChanges();
            }

            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            U.Status = 1;
            //Request Table
            R.RequestTypeId = 2; // 2 stands for patient in RequestType table
            R.CreatedDate = DateTime.Now;
            R.FirstName = vdcp.FirstName;
            R.LastName = vdcp.LastName;
            R.Email = vdcp.Email;
            R.PhoneNumber = vdcp.Mobile;
            R.Status = U.Status;
            R.ConfirmationNumber = R.PhoneNumber;
            R.CreatedUserId = U.UserId;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.FirstName = vdcp.FirstName;
            RC.LastName = vdcp.LastName;
            RC.PhoneNumber = vdcp.Mobile;
            RC.Address = vdcp.Street + ", " + vdcp.City + ", " + vdcp.State + ", " + vdcp.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcp.Notes;
            RC.Email = vdcp.Email;
            RC.StrMonth = (vdcp.DOB.Month).ToString();
            RC.IntDate = vdcp.DOB.Day;
            RC.IntYear = vdcp.DOB.Year;
            RC.Street = vdcp.Street;
            RC.City = vdcp.City;
            RC.State = vdcp.State;
            RC.ZipCode = vdcp.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();
        }
    }
}
