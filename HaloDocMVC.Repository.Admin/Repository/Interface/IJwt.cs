using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IJwt
    {
        string GenerateJWTAuthetication(UserInfo userinfo);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
    }
}
