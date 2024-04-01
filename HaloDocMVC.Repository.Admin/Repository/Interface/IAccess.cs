using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IAccess
    {
        public List<AccessMenu> AccessIndex();

        public bool CreateAccessPost(AccessMenu am, string id);
    }
}
