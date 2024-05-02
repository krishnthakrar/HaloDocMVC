using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Entity.DataModels;

[Table("TimeSheetDetail")]
public partial class TimeSheetDetail
{
    [Key]
    public int TimeSheetDetailId { get; set; }

    public int? TimeSheetId { get; set; }

    public DateOnly? Date { get; set; }

    public int? OnCallHours { get; set; }

    public int? TotalHours { get; set; }

    public bool? IsWeekendOrHoliDay { get; set; }

    public int? NoOfHouseCall { get; set; }

    public int? NoOfPhoneconsult { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimeSheetDetailCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimeSheetDetailModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimeSheetId")]
    [InverseProperty("TimeSheetDetails")]
    public virtual TimeSheet? TimeSheet { get; set; }

    [InverseProperty("TimeSheetDetail")]
    public virtual ICollection<TimeSheetReceipt> TimeSheetReceipts { get; set; } = new List<TimeSheetReceipt>();
}
