using System.IdentityModel.Tokens.Jwt;

namespace HaloDocMVC.Models
{
    public static class CredentialValue
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CredentialValue()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }
        public static string? role()
        {
            string cookieValue;
            string role = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                role = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "Role").Value;
            }
            return role;
        }


        public static string? UserName()
        {
            string cookieValue;
            string UserName = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                UserName = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserName").Value;
            }
            return UserName;
        }

        public static string? UserId()
        {
            string cookieValue;
            string UserID = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            }
            return UserID;
        }

        public static string? ID()
        {
            string cookieValue;
            string UserID = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "AspNetUserId").Value;
            }
            return UserID;
        }

        public static string? CurrentStatus()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["Status"];
            return Status;
        }

        public static int RoleId()
        {
            string cookieValue;
            int RoleId = 0;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                RoleId = int.Parse(DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "RoleId").Value);
            }
            return RoleId;
        }
    }
}
