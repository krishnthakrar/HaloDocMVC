﻿using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HaloDocMVC.Controllers
{
    [AttributeUsage(AttributeTargets.All)]
    public class ProviderAccess : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public ProviderAccess(string role)
        {
            _role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwt>();
            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("../Login/Index");
                return;
            }
            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];
            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("../Login/Index");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("../Login/Index");
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roles.Value != _role)
            {
                filterContext.Result = new RedirectResult("../Home/AuthError");

            }

        }
    }
}
