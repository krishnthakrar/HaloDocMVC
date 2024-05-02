using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Entity.DataModels;

[Table("TimeSheetReceipt")]
public partial class TimeSheetReceipt
{
    [Key]
    public int TimeSheetReceiptId { get; set; }

    public int? TimeSheetDetailId { get; set; }

    [StringLength(100)]
    public string? ItemName { get; set; }

    public int? Amount { get; set; }

    [StringLength(200)]
    public string? BillName { get; set; }

    [StringLength(50)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimeSheetReceiptCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimeSheetReceiptModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimeSheetDetailId")]
    [InverseProperty("TimeSheetReceipts")]
    public virtual TimeSheetDetail? TimeSheetDetail { get; set; }
}
