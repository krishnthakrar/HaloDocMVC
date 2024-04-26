using HaloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository.Interface
{
    public interface IScheduling
    {
        public DayWiseScheduling Daywise(int regionid, DateTime currentDate);

        public WeekWiseScheduling Weekwise(int regionid, DateTime currentDate);

        public MonthWiseScheduling Monthwise(int regionid, DateTime currentDate);

        public MonthWiseScheduling MonthwisePhysician(DateTime currentDate, int id);

        public void AddShift(SchedulingData model, List<string?>? chk, string adminId);
        
        public SchedulingData ViewShift(int shiftdetailid);
        
        public void ViewShiftreturn(SchedulingData modal);
        
        public bool ViewShiftSave(SchedulingData modal, string id);
        
        public bool ViewShiftDelete(SchedulingData modal, string id);
        
        public Task<List<ProviderMenu>> PhysicianOnCall(int? region);
        
        public SchedulingData GetAllNotApprovedShift(int? regionId, SchedulingData sd);
        
        public Task<bool> DeleteShift(string s, string AdminID);
        
        public Task<bool> UpdateStatusShift(string s, string AdminID);
    }
}
