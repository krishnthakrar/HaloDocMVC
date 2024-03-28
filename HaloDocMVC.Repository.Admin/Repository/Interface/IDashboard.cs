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
        public DashboardList GetPatientRequest(string id, DashboardList listdata);

        public ViewDataUserProfile UserProfile(int id);

        public void EditProfile(ViewDataUserProfile vdup, int id);

        public Task<ViewDocument> ViewDocumentList(int? id, ViewDocument viewDocument);

        public void UploadDoc(int id, IFormFile? UploadFile);
    }
}
