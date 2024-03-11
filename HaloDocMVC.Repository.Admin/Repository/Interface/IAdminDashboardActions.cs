using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IAdminDashboardActions
    {
        public ViewDataViewCase NewRequestData(int? RId, int? RTId, int? Status);

        public ViewDataViewCase Edit(ViewDataViewCase vdvc, int? RId, int? RTId, int? Status);

        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);

        public bool CancelCase(int RequestID, string Note, string CaseTag);
        
        public bool BlockCase(int RequestID, string Note);

        public bool ClearCase(int RequestID);

        public ViewDataViewNotes GetNotesByID(int id);

        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);

        public Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);

        public Task<ViewDataViewDocuments> GetDocumentByRequest(int? id);

        public Boolean SaveDoc(int Requestid, IFormFile file);

        public Task<bool> DeleteDocumentByRequest(string ids);

        public HealthProfessional SelectProfessionalByID(int VendorID);

        public bool SendOrder(ViewDataViewOrders data);
    }
}
