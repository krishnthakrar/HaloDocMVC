using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Login : ILogin
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HaloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public Login(HaloDocContext context, IHttpContextAccessor httpContextAccessor, EmailConfiguration emailConfig)
        {
            this.httpContextAccessor = httpContextAccessor;
            _context = context;
            _emailConfig = emailConfig;
        }

        #region CheckAccess
        public async Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email && u.PasswordHash == aspNetUser.PasswordHash);
            UserInfo admin = new UserInfo();
            if (user != null)
            {
                var data = _context.AspNetUserRoles.FirstOrDefault(E => E.UserId == user.Id);
                var datarole = _context.AspNetRoles.FirstOrDefault(e => e.Id == data.RoleId);
                admin.UserName = user.UserName;
                admin.FirstName = admin.FirstName ?? string.Empty;
                admin.LastName = admin.LastName ?? string.Empty;
                admin.Role = datarole.Name;
                admin.AspNetUserId = user.Id;
                if (admin.Role == "Admin")
                {
                    var admindata = _context.Admins.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.AdminId;
                    admin.RoleId = (int)admindata.RoleId;
                }
                else if (admin.Role == "Patient")
                {
                    var admindata = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.UserId;
                }
                else
                {
                    var admindata = _context.Physicians.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.PhysicianId;
                    admin.RoleId = (int)admindata.RoleId;
                }
                return admin;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region isAccessGranted
        public bool isAccessGranted(int roleId, string menuName)
        {
            // Get the list of menu IDs associated with the role
            IQueryable<int> menuIds = _context.RoleMenus
                                            .Where(e => e.RoleId == roleId)
                                            .Select(e => e.MenuId);

            // Check if any menu with the given name exists in the list of menu IDs
            bool accessGranted = _context.Menus
                                         .Any(e => menuIds.Contains(e.MenuId) && e.Name == menuName);

            return accessGranted;
        }
        #endregion

        #region ResetLink
        public bool SendResetLink(String Email)
        {
            var isexist = _context.Users.FirstOrDefault(x => x.Email == Email);
            if (isexist != null) {
                var agreementUrl = "https://localhost:44348/Login/ResetPassword?Email=" + Email + "&Datetime=" + DateTime.Now;
                _emailConfig.SendMail(Email, "Reset your password", $"To reset your password <a href='{agreementUrl}'>Click here..</a>");

                EmailLog E = new();
                E.SubjectName = "Reset your password";
                E.EmailTemplate = "Reset your password";
                E.EmailId = Email;
                E.RoleId = 2;
                E.CreateDate = DateTime.Now;
                E.SentDate = DateTime.Now;
                E.IsEmailSent = new BitArray(1);
                E.IsEmailSent[0] = true;
                E.Action = 5;
                _context.EmailLogs.Add(E);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region ResetLinkPatient
        public bool SendResetLinkPatient(String Email)
        {
            var isexist = _context.Users.FirstOrDefault(x => x.Email == Email);
            if (isexist != null)
            {
                var agreementUrl = "https://localhost:44348/PatientLogin/ResetPassword?Email=" + Email + "&Datetime=" + DateTime.Now;
                _emailConfig.SendMail(Email, "Reset your password", $"To reset your password <a href='{agreementUrl}'>Click here..</a>");

                EmailLog E = new();
                E.SubjectName = "Reset your password";
                E.EmailTemplate = "Reset your password";
                E.EmailId = Email;
                E.RoleId = 3;
                E.CreateDate = DateTime.Now;
                E.SentDate = DateTime.Now;
                E.IsEmailSent = new BitArray(1);
                E.IsEmailSent[0] = true;
                E.Action = 5;
                _context.EmailLogs.Add(E);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region CreateAccount
        public bool CreateAccount(string Email, string Password)
        {
            try
            {
                AspNetUser A = new();
                //AspNetUser Table
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = Email;
                A.PasswordHash = Password;
                A.Email = Email;
                A.CreatedDate = DateTime.Now;
                _context.Add(A);

                var U = _context.RequestClients.FirstOrDefault(m => m.Email == Email);
                var User = new User
                {
                    AspNetUserId = A.Id,
                    FirstName = U.FirstName,
                    LastName = U.LastName,
                    Mobile = U.PhoneNumber,
                    IntDate = U.IntDate,
                    IntYear = U.IntYear,
                    StrMonth = U.StrMonth,
                    Email = Email,
                    CreatedBy = A.Id,
                    CreatedDate = DateTime.Now,
                    IsRequestWithEmail = new BitArray(1),
                    Status = 1
                };
                _context.Users.Add(User);
                _context.SaveChanges();

                var aspnetuserroles = new AspNetUserRole();
                aspnetuserroles.UserId = User.AspNetUserId;
                aspnetuserroles.RoleId = "3";
                _context.AspNetUserRoles.Add(aspnetuserroles);
                _context.SaveChanges();

                var rc = _context.RequestClients.Where(e => e.Email == Email).ToList();

                foreach (var r in rc)
                {
                    _context.Requests.Where(n => n.RequestId == r.RequestId)
                   .ExecuteUpdate(s => s.SetProperty(
                       n => n.UserId,
                       n => User.UserId));
                }
                if (rc.Count > 0)
                {
                    User.IntDate = rc[0].IntDate;
                    User.IntYear = rc[0].IntYear;
                    User.StrMonth = rc[0].StrMonth;
                    _context.Users.Update(User);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
        #endregion
    }
}
