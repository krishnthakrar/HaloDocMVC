using HaloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IDashboard
    {
        public List<DashboardList> DashboardList(int ? id);

        public ViewDataUserProfile UserProfile(int id);

        public void EditProfile(ViewDataUserProfile vdup, int id);

        public List<ViewDocument> ViewDocumentList(int? id);

        public void UploadDoc(int RequestId, IFormFile? UploadFile);
    }
}
