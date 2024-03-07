using HaloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class ViewDataViewNotes
    {
        public int? RequestNotesId { get; set; }

        public int? RequestId { get; set; }

        public string? StrMonth { get; set; }

        public int? IntYear { get; set; }

        public int? IntDate { get; set; }

        public string? PhysicianNotes { get; set; }

        public string? PatientNotes { get; set; }

        public string? AdminNotes { get; set; }

        public string? CreatedBy { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public short Status { get; set; }

        public string? IP { get; set; }

        public string? AdministrativeNotes { get; set; }

        public virtual Request Request { get; set; } = null!;

        public List<TransferNotes> TransferNotes { get; set; } = null!;

        public List<TransferNotes> CancelNotes { get; set; } = null!;

        public List<TransferNotes> Cancel { get; set; } = null!;

        public List<TransferNotes> CancelByPhysician { get; set; } = null!;
    }
}