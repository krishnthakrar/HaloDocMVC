﻿using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IButtons
    {
        public void CreateRequest(ViewDataCreatePatient vdcp);

        public Boolean SendLink(string FirstName, string LastName, string Email);

        List<AdminDashboardList> Export(string status);
    }
}