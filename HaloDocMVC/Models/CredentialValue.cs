﻿namespace HaloDocMVC.Models
{
    public class CredentialValue
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CredentialValue()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public static string? UserName()
        {
            string? UserName = null;
            if (_httpContextAccessor.HttpContext.Session.GetString("UserName") != null)
            {
                UserName = _httpContextAccessor.HttpContext.Session.GetString("UserName").ToString();
            }
            return UserName;
        }

        public static string? UserID()
        {
            string? UserID = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("UserID") != null)
            {
                UserID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

            }
            return UserID;
        }
        public static string? RoleID()
        {
            string? RoleID = null;

            if (_httpContextAccessor.HttpContext.Session.GetString("RoleId") != null)
            {
                RoleID = _httpContextAccessor.HttpContext.Session.GetString("RoleId");

            }
            return RoleID;
        }
    }
}
