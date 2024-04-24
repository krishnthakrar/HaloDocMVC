using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    public string? FirstName { get; set; }

    [StringLength(20)]
    public string? LastName { get; set; }

    public int? CityId { get; set; }

    public int? Age { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(15)]
    public string? PhoneNo { get; set; }

    [StringLength(8)]
    public string? Gender { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Users")]
    public virtual City? CityNavigation { get; set; }
}
