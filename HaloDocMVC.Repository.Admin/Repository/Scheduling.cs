﻿using HaloDocMVC.Entity.DataContext;
using HaloDocMVC.Entity.DataModels;
using HaloDocMVC.Entity.Models;
using HaloDocMVC.Repository.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocMVC.Repository.Admin.Repository
{
    public class Scheduling : IScheduling
    {
        private readonly HaloDocContext _context;

        public Scheduling(HaloDocContext context)
        {
            _context = context;
        }

        #region AddShift
        public void AddShift(SchedulingData model, List<string?>? chk, string adminId)
        {
            var shiftid = _context.Shifts.Where(u => u.PhysicianId == model.physicianid).Select(u => u.ShiftId).ToList();
            if (shiftid.Count() > 0)
            {
                foreach (var obj in shiftid)
                {
                    var shiftdetailchk = _context.ShiftDetails.Where(u => u.ShiftId == obj && u.ShiftDate == model.shiftdate).ToList();
                    if (shiftdetailchk.Count() > 0)
                    {
                        foreach (var item in shiftdetailchk)
                        {
                            if ((model.starttime >= item.StartTime && model.starttime <= item.EndTime) || (model.endtime >= item.StartTime && model.endtime <= item.EndTime))
                            {
                                //TempData["error"] = "Shift is already assigned in this time";
                                return;
                            }
                        }
                    }
                }
            }
            Shift shift = new Shift
            {
                PhysicianId = model.physicianid,
                StartDate = DateOnly.FromDateTime(model.shiftdate),
                RepeatUpto = model.repeatcount,
                CreatedDate = DateTime.Now,
                CreatedBy = adminId,
            };
            foreach (var obj in chk)
            {
                shift.WeekDays += obj;
            }
            if (model.repeatcount > 0)
            {
                shift.IsRepeat = new BitArray(new[] { true });
            }
            else
            {
                shift.IsRepeat = new BitArray(new[] { false });
            }
            _context.Shifts.Add(shift);
            _context.SaveChanges();
            
            DateTime curdate = model.shiftdate;
            ShiftDetail shiftdetail = new ShiftDetail();
            shiftdetail.ShiftId = shift.ShiftId;
            shiftdetail.ShiftDate = curdate;
            shiftdetail.RegionId = model.regionid;
            shiftdetail.StartTime = model.starttime;
            shiftdetail.EndTime = model.endtime;
            shiftdetail.IsDeleted = new BitArray(new[] { false });
            _context.ShiftDetails.Add(shiftdetail);
            _context.SaveChanges();
            ShiftDetailRegion shiftregionnews = new ShiftDetailRegion
            {
                ShiftDetailId = shiftdetail.ShiftDetailId,
                RegionId = model.regionid,
                IsDeleted = new BitArray(new[] { false })
            };
            _context.ShiftDetailRegions.Add(shiftregionnews);
            _context.SaveChanges();
            var dayofweek = model.shiftdate.DayOfWeek.ToString();
            int valueforweek;
            if (dayofweek == "Sunday")
            {
                valueforweek = 0;
            }
            else if (dayofweek == "Monday")
            {
                valueforweek = 1;
            }
            else if (dayofweek == "Tuesday")
            {
                valueforweek = 2;
            }
            else if (dayofweek == "Wednesday")
            {
                valueforweek = 3;
            }
            else if (dayofweek == "Thursday")
            {
                valueforweek = 4;
            }
            else if (dayofweek == "Friday")
            {
                valueforweek = 5;
            }
            else
            {
                valueforweek = 6;
            }

            if (shift.IsRepeat[0] == true)
            {
                for (int j = 0; j < shift.WeekDays.Count(); j++)
                {
                    var z = shift.WeekDays;
                    var p = shift.WeekDays.ElementAt(j).ToString();
                    int ele = Int32.Parse(p);
                    int x;
                    if (valueforweek > ele)
                    {
                        x = 6 - valueforweek + 1 + ele;
                    }
                    else
                    {
                        x = ele - valueforweek;
                    }
                    if (x == 0)
                    {
                        x = 7;
                    }
                    DateTime newcurdate = model.shiftdate.AddDays(x);
                    for (int i = 0; i < model.repeatcount; i++)
                    {
                        ShiftDetail shiftdetailnew = new ShiftDetail
                        {
                            ShiftId = shift.ShiftId,
                            ShiftDate = newcurdate,
                            RegionId = model.regionid,
                            StartTime = model.starttime,
                            EndTime = model.endtime,

                            IsDeleted = new BitArray(new[] { false })
                        };
                        _context.ShiftDetails.Add(shiftdetailnew);
                        _context.SaveChanges();
                        ShiftDetailRegion shiftregionnew = new ShiftDetailRegion
                        {
                            ShiftDetailId = shiftdetailnew.ShiftDetailId,
                            RegionId = model.regionid,
                            IsDeleted = new BitArray(new[] { false })
                        };
                        _context.ShiftDetailRegions.Add(shiftregionnew);
                        _context.SaveChanges();
                        newcurdate = newcurdate.AddDays(7);
                    }
                }

            }
        }
        #endregion

        #region EditShift
        public SchedulingData ViewShift(int shiftdetailid)
        {
            SchedulingData modal = new SchedulingData();
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == shiftdetailid);

            if (shiftdetail != null)
            {
                _context.Entry(shiftdetail)
                    .Reference(s => s.Shift)
                    .Query()
                    .Include(s => s.Physician)
                    .Load();
            }

            modal.regionid = (int)shiftdetail.RegionId;
            modal.physicianname = shiftdetail.Shift.Physician.FirstName + " " + shiftdetail.Shift.Physician.LastName;
            modal.modaldate = shiftdetail.ShiftDate.ToString("yyyy-MM-dd");
            modal.starttime = shiftdetail.StartTime;
            modal.endtime = shiftdetail.EndTime;
            modal.shiftdetailid = shiftdetailid;
            return modal;
        }

        public void ViewShiftreturn(SchedulingData modal)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
        }

        public bool ViewShiftSave(SchedulingData modal, string id)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail != null)
            {
                shiftdetail.ShiftDate = modal.shiftdate;
                shiftdetail.StartTime = modal.starttime;
                shiftdetail.EndTime = modal.endtime;
                shiftdetail.ModifiedBy = id;
                shiftdetail.ModifiedDate = DateTime.Now;
                _context.ShiftDetails.Update(shiftdetail);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ViewShiftDelete(SchedulingData modal, string id)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            var shiftdetailRegion = _context.ShiftDetailRegions.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            string adminname = id;

            shiftdetail.IsDeleted = new BitArray(new[] { true });
            shiftdetail.ModifiedDate = DateTime.Now;
            shiftdetail.ModifiedBy = adminname;
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();

            shiftdetailRegion.IsDeleted = new BitArray(new[] { true });
            _context.ShiftDetailRegions.Update(shiftdetailRegion);
            _context.SaveChanges();

            return true;
        }
        #endregion

        #region PhysicianOnCall
        public async Task<List<ProviderMenu>> PhysicianOnCall(int? region)
        {
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTimeOfDay = TimeOnly.FromDateTime(DateTime.Now);

            List<ProviderMenu> pm = await (from r in _context.Physicians
                                           where r.IsDeleted == new BitArray(1)
                                           select new ProviderMenu
                                           {
                                               CreatedDate = r.CreatedDate,
                                               PhysicianId = r.PhysicianId,
                                               Address1 = r.Address1,
                                               Address2 = r.Address2,
                                               AdminNotes = r.AdminNotes,
                                               AltPhone = r.AltPhone,
                                               BusinessName = r.BusinessName,
                                               BusinessWebsite = r.BusinessWebsite,
                                               City = r.City,
                                               FirstName = r.FirstName,
                                               LastName = r.LastName,
                                               Status = r.Status,
                                               Email = r.Email,
                                               Photo = r.Photo

                                           }).ToListAsync();
            if (region != null)
            {
                pm = await (from pr in _context.PhysicianRegions
                            join ph in _context.Physicians
                            on pr.PhysicianId equals ph.PhysicianId into rGroup
                            from r in rGroup.DefaultIfEmpty()
                            where pr.RegionId == region && r.IsDeleted == new BitArray(1)
                            select new ProviderMenu
                            {
                                CreatedDate = r.CreatedDate,
                                PhysicianId = r.PhysicianId,
                                Address1 = r.Address1,
                                Address2 = r.Address2,
                                AdminNotes = r.AdminNotes,
                                AltPhone = r.AltPhone,
                                BusinessName = r.BusinessName,
                                BusinessWebsite = r.BusinessWebsite,
                                City = r.City,
                                FirstName = r.FirstName,
                                LastName = r.LastName,
                                Status = r.Status,
                                Email = r.Email,
                                Photo = r.Photo
                            })
                            .ToListAsync();
            }

            foreach (var item in pm)
            {
                List<int> shiftIds = await (from s in _context.Shifts
                                            where s.PhysicianId == item.PhysicianId
                                            select s.ShiftId).ToListAsync();
                foreach (var shift in shiftIds)
                {
                    var shiftDetail = (from sd in _context.ShiftDetails
                                       where sd.ShiftId == shift &&
                                             sd.ShiftDate.Date == currentDateTime.Date &&
                                             sd.StartTime <= currentTimeOfDay &&
                                             currentTimeOfDay <= sd.EndTime
                                       select sd).FirstOrDefault();
                    if (shiftDetail != null)
                    {
                        item.OnCallStatus = 1;
                    }
                }
            }
            return pm;
        }
        #endregion

        #region GetAllNotApprovedShift
        public SchedulingData GetAllNotApprovedShift(int? regionId, SchedulingData schd)
        {

            List<SchedulingData> ss = (from s in _context.Shifts
                                             join pd in _context.Physicians
                                             on s.PhysicianId equals pd.PhysicianId
                                             join sd in _context.ShiftDetails
                                             on s.ShiftId equals sd.ShiftId into shiftGroup
                                             from sd in shiftGroup.DefaultIfEmpty()
                                             join rg in _context.Regions
                                             on sd.RegionId equals rg.RegionId
                                             where (regionId == null || regionId == 0 || sd.RegionId == regionId) && sd.Status == 0 && sd.IsDeleted == new BitArray(1)
                                             select new SchedulingData
                                             {
                                                 regionid = (int)sd.RegionId,
                                                 RegionName = rg.Name,
                                                 shiftdetailid = sd.ShiftDetailId,
                                                 status = sd.Status,
                                                 starttime = sd.StartTime,
                                                 endtime = sd.EndTime,
                                                 physicianid = s.PhysicianId,
                                                 physicianname = pd.FirstName + ' ' + pd.LastName,
                                                 shiftdate = sd.ShiftDate
                                             })
                                .ToList();
            if (schd.IsAscending == true)
            {
                ss = schd.SortedColumn switch
                {
                    "PhysicianName" => ss.OrderBy(x => x.physicianname).ToList(),
                    "ShiftDate" => ss.OrderBy(x => x.shiftdate).ToList(),
                    "StartTime" => ss.OrderBy(x => x.starttime).ToList(),
                    "RegionName" => ss.OrderBy(x => x.regionid).ToList(),
                    _ => ss.OrderBy(x => x.physicianname).ToList()
                };
            }
            else
            {
                ss = schd.SortedColumn switch
                {
                    "PhysicianName" => ss.OrderByDescending(x => x.physicianname).ToList(),
                    "ShiftDate" => ss.OrderByDescending(x => x.shiftdate).ToList(),
                    "StartTime" => ss.OrderByDescending(x => x.starttime).ToList(),
                    "RegionName" => ss.OrderByDescending(x => x.regionid).ToList(),
                    _ => ss.OrderByDescending(x => x.physicianname).ToList()
                };
            }
            int totalItemCount = ss.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)schd.PageSize);
            List<SchedulingData> list = ss.Skip((schd.CurrentPage - 1) * schd.PageSize).Take(schd.PageSize).ToList();

            SchedulingData model = new()
            {
                SD = list,
                CurrentPage = schd.CurrentPage,
                TotalPages = totalPages,
                PageSize = schd.PageSize,
                IsAscending = schd.IsAscending,
                SortedColumn = schd.SortedColumn
            };

            return model;
        }
        #endregion

        #region DeleteShift
        public async Task<bool> DeleteShift(string s, string AdminID)
        {
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    ShiftDetail sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftDetailId == i);
                    if (sd != null)
                    {
                        sd.IsDeleted[0] = true;
                        sd.ModifiedBy = AdminID;
                        sd.ModifiedDate = DateTime.Now;
                        _context.ShiftDetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region UpdateStatusShift
        public async Task<bool> UpdateStatusShift(string s, string AdminID)
        {
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    ShiftDetail sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftDetailId == i);
                    if (sd != null)
                    {
                        sd.Status = (short)(sd.Status == 1 ? 0 : 1);
                        sd.ModifiedBy = AdminID;
                        sd.ModifiedDate = DateTime.Now;
                        _context.ShiftDetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
