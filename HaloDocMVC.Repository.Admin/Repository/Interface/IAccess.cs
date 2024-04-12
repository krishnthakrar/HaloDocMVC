﻿using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IAccess
    {
        public AccessMenu AccessIndex(AccessMenu am);

        public bool CreateAccessPost(AccessMenu am, string id);

        public bool DeleteAccess(int? id);

        public AccessMenu EditAccess(int? id);

        public bool EditAccessPost(AccessMenu am);

        public UserAccess GetAllUserDetails(int? User, UserAccess ua);

        public bool CreateAdmin(ViewAdminProfile vap, string? id);
    }
}
