using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IAdminDashboardActions
    {
        public ViewDataViewCase NewRequestData(int? RId, int? RTId);

        public ViewDataViewCase Edit(ViewDataViewCase vdvc, int? RId, int? RTId);

        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);

        public bool CancelCase(int RequestID, string Note, string CaseTag);
    }
}
