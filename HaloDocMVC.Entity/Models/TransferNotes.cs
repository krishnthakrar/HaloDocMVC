using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Entity.Models
{
    public class TransferNotes
    {
        public int RequestStatusLogId { get; set; }

        public int RequestId { get; set; }

        public int? PhysicianId { get; set; }

        public int? TransToPhysicianId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? Notes { get; set; }

        public short Status { get; set; }

        public string? TransPhysician { get; set; }

        public string? Admin { get; set; }

        public string? Physician { get; set; }

        public string TransferNote => $"{Admin} transferred <b> {Physician} </b> to <b> {TransPhysician} </b> on {CreatedDate}: <b>{Notes}</b>";

        public BitArray? TransToAdmin { get; set; }
    }
}
