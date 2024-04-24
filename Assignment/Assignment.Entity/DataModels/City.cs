using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("City")]
public partial class City
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("CityNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
