using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IRecords
    {
        RecordsModel GetFilteredSearchRecords(RecordsModel rm);

        bool DeleteRequest(int? RequestId);

        RecordsModel BlockHistory(RecordsModel rm);

        bool Unblock(int RequestId, string id);

        RecordsModel GetFilteredPatientHistory(RecordsModel rm);

        PaginatedViewModel PatientRecord(int UserId, PaginatedViewModel data);

        public RecordsModel GetFilteredSMSLogs(RecordsModel rm);

        public RecordsModel GetFilteredEmailLogs(RecordsModel rm);
    }
}
