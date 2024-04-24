using System;
using System.Collections.Generic;
using Assignment.Entity.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataContext;

public partial class UMSContext : DbContext
{
    public UMSContext()
    {
    }

    public UMSContext(DbContextOptions<UMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=Krishn@1303;Server=localhost;Port=5432;Database=UMS;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("City_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(11L, null, null, null, null, null);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(11L, null, null, null, null, null);

            entity.HasOne(d => d.CityNavigation).WithMany(p => p.Users).HasConstraintName("User_CityId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
