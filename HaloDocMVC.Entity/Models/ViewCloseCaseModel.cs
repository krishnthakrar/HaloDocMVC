using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HaloDocMVC.Entity.Models.ViewDataViewDocuments;

namespace HaloDocMVC.Entity.Models
{
    public class ViewCloseCaseModel
    {
        public List<Documents> DocumentsList { get; set; } = null;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public int RequestId { get; set; }
        public int RequestWiseFileId { get; set; }
        public string RC_FirstName { get; set; }
        public string RC_LastName { get; set; }
        public string RC_Email { get; set; }
        public DateTime RC_DOB { get; set; }
        public string RC_PhoneNumber { get; set; }
        public int RequestClientId { get; set; }
    }
}
