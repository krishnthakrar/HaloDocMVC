using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class Constant
    {
        public enum ListType
        {
            Business = 1,
            Patient,
            Family,
            Concierge
        }
        public enum AdminDashStatus
        {
            New = 1,
            Pending,
            Active,
            Conclude,
            ToClose,
            UnPaid
        }
        public enum Status
        {
            Unassigned = 1,
            Accepted, Cancelled, MDEnRoute, MDONSite, Conclude, CancelledByPatients, Closed, Unpaid, Clear,Block
        }
        public enum AdminStatus
        {
            Active = 1, 
            Pending, 
            NotActive
        }

        public enum OnCallStatus
        {
            UnAvailable = 0,
            Available
        }

        public enum AccountType
        {
            Admin = 1,
            Provider,
            Patient,
        }

        public enum EmailAction
        {
            Sendorder = 1,
            Request,
            SendLink,
            SendAgreement,
            Forgot,
            NewRegistration,
            contact
        }
    }
}
