using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IPatientRequest
    {
        public void CreatePatient(ViewDataCreatePatient vdcp);

        public void CreateFriend(ViewDataCreateFriend vdcf);

        public void CreateConcierge(ViewDataCreateConcierge vdcc);

        public void CreatePartner(ViewDataCreateBusiness vdcb);

        public ViewDataCreatePatient ViewMe(int id);

        public void CreateMe(ViewDataCreatePatient vdcp);

        public void CreateSomeOneElse(ViewDataCreateSomeOneElse vdcs);
    }
}
