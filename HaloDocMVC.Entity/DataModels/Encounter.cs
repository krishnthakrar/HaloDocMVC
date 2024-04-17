using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Entity.DataModels;

[Table("Encounter")]
public partial class Encounter
{
    [Key]
    public int EncounterId { get; set; }

    public int RequestId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DateOfService { get; set; }

    [StringLength(500)]
    public string? MedicalHistory { get; set; }

    [StringLength(128)]
    public string? Injury { get; set; }

    [StringLength(200)]
    public string? Medications { get; set; }

    [StringLength(200)]
    public string? Allergies { get; set; }

    [StringLength(10)]
    public string? Temp { get; set; }

    [Column("HR")]
    [StringLength(20)]
    public string? Hr { get; set; }

    [Column("RR")]
    [StringLength(20)]
    public string? Rr { get; set; }

    [StringLength(20)]
    public string? BloodPressure { get; set; }

    [StringLength(20)]
    public string? O2 { get; set; }

    [StringLength(100)]
    public string? Pain { get; set; }

    [StringLength(200)]
    public string? Heent { get; set; }

    [Column("CV")]
    [StringLength(50)]
    public string? Cv { get; set; }

    [StringLength(50)]
    public string? Chest { get; set; }

    [Column("ABD")]
    [StringLength(50)]
    public string? Abd { get; set; }

    [StringLength(100)]
    public string? Extr { get; set; }

    [StringLength(100)]
    public string? Skin { get; set; }

    [StringLength(100)]
    public string? Neuro { get; set; }

    [StringLength(200)]
    public string? Other { get; set; }

    [StringLength(200)]
    public string? Diagnosis { get; set; }

    [StringLength(200)]
    public string? Treatment { get; set; }

    [StringLength(200)]
    public string? MedicationsDispensed { get; set; }

    [StringLength(200)]
    public string? Procedures { get; set; }

    [StringLength(100)]
    public string? Followup { get; set; }

    public bool? IsFinalized { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Encounters")]
    public virtual Request Request { get; set; } = null!;
}
