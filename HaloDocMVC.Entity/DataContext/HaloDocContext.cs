﻿using System;
using System.Collections.Generic;
using HaloDocMVC.Entity.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HaloDocMVC.Entity.DataContext;

public partial class HaloDocContext : DbContext
{
    public HaloDocContext()
    {
    }

    public HaloDocContext(DbContextOptions<HaloDocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminRegion> AdminRegions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<BlockRequest> BlockRequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<CaseTag> CaseTags { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<Encounter> Encounters { get; set; }

    public virtual DbSet<HealthProfessional> HealthProfessionals { get; set; }

    public virtual DbSet<HealthProfessionalType> HealthProfessionalTypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianLocation> PhysicianLocations { get; set; }

    public virtual DbSet<PhysicianNotification> PhysicianNotifications { get; set; }

    public virtual DbSet<PhysicianPayrate> PhysicianPayrates { get; set; }

    public virtual DbSet<PhysicianRegion> PhysicianRegions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestBusiness> RequestBusinesses { get; set; }

    public virtual DbSet<RequestClient> RequestClients { get; set; }

    public virtual DbSet<RequestClosed> RequestCloseds { get; set; }

    public virtual DbSet<RequestConcierge> RequestConcierges { get; set; }

    public virtual DbSet<RequestNote> RequestNotes { get; set; }

    public virtual DbSet<RequestStatusLog> RequestStatusLogs { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<RequestWiseFile> RequestWiseFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<ShiftDetailRegion> ShiftDetailRegions { get; set; }

    public virtual DbSet<Smslog> Smslogs { get; set; }

    public virtual DbSet<TimeSheet> TimeSheets { get; set; }

    public virtual DbSet<TimeSheetDetail> TimeSheetDetails { get; set; }

    public virtual DbSet<TimeSheetDetailReimbursement> TimeSheetDetailReimbursements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("User ID = postgres;Password=Krishn@1303;Server=localhost;Port=5432;Database=HaloDoc;Integrated Security=true;Pooling=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("Admin_pkey");

            entity.Property(e => e.AdminId).HasIdentityOptions(null, 2L, null, null, null, null);

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.AdminAspNetUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AspNetUserId");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AdminCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AdminModifiedByNavigations).HasConstraintName("ModifiedBy");

            entity.HasOne(d => d.Region).WithMany(p => p.Admins).HasConstraintName("Region");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins).HasConstraintName("Role");
        });

        modelBuilder.Entity<AdminRegion>(entity =>
        {
            entity.HasKey(e => e.AdminRegionId).HasName("AdminRegion_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminRegion_AdminId_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminRegion_RegionId_fkey");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoles_pkey");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("AspNetUserRoles_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AspNetUserRoles_UserId_fkey");
        });

        modelBuilder.Entity<BlockRequest>(entity =>
        {
            entity.HasKey(e => e.BlockRequestId).HasName("BlockRequests_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.BlockRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BlockRequests_RequestId_fkey");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.BusinessId).HasName("Business_pkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BusinessCreatedByNavigations).HasConstraintName("Business_CreatedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.BusinessModifiedByNavigations).HasConstraintName("Business_ModifiedBy_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Businesses).HasConstraintName("Business_RegionId_fkey");
        });

        modelBuilder.Entity<CaseTag>(entity =>
        {
            entity.HasKey(e => e.CaseTagId).HasName("CaseTag_pkey");
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.ConciergeId).HasName("Concierge_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Concierges).HasConstraintName("Concierge_RegionId_fkey");
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.EmailLogId).HasName("EmailLog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.EmailLogs).HasConstraintName("EmailLog_AdminId_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.EmailLogs).HasConstraintName("EmailLog_PhysicianId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.EmailLogs).HasConstraintName("EmailLog_RequestId_fkey");
        });

        modelBuilder.Entity<Encounter>(entity =>
        {
            entity.HasKey(e => e.EncounterId).HasName("Encounter_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Encounters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Encounter_RequestId_fkey");
        });

        modelBuilder.Entity<HealthProfessional>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("HealthProfessionals_pkey");

            entity.Property(e => e.VendorId).HasIdentityOptions(21L, null, null, null, null, null);

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.HealthProfessionals).HasConstraintName("HealthProfessionals_Profession_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.HealthProfessionals).HasConstraintName("HealthProfessionals_RegionId_fkey");
        });

        modelBuilder.Entity<HealthProfessionalType>(entity =>
        {
            entity.HasKey(e => e.HealthProfessionalId).HasName("HealthProfessionalType_pkey");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("Menu_pkey");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OrderDetails_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.OrderDetails).HasConstraintName("OrderDetails_RequestId_fkey");

            entity.HasOne(d => d.Vendor).WithMany(p => p.OrderDetails).HasConstraintName("OrderDetails_VendorId_fkey");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("Physician_pkey");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.PhysicianAspNetUsers).HasConstraintName("Physician_AspNetUserId_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PhysicianCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Physician_CreatedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PhysicianModifiedByNavigations).HasConstraintName("Physician_ModifiedBy_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicians).HasConstraintName("Physician_RegionId_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Physicians).HasConstraintName("Physician_RoleId_fkey");
        });

        modelBuilder.Entity<PhysicianLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PhysicianLocation_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianLocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianLocation_PhysicianId_fkey");
        });

        modelBuilder.Entity<PhysicianNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PhysicianNotification_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianNotification_PhysicianId_fkey");
        });

        modelBuilder.Entity<PhysicianPayrate>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("PhysicianPayrate_pkey");

            entity.Property(e => e.PhysicianId).ValueGeneratedNever();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PhysicianPayrateCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianPayrate_CreatedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PhysicianPayrateModifiedByNavigations).HasConstraintName("PhysicianPayrate_ModifiedBy_fkey");

            entity.HasOne(d => d.Physician).WithOne(p => p.PhysicianPayrate)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianPayrate_PhysicianId_fkey");
        });

        modelBuilder.Entity<PhysicianRegion>(entity =>
        {
            entity.HasKey(e => e.PhysicianRegionId).HasName("PhysicianRegion_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianRegion_PhysicianId_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianRegion_RegionId_fkey");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("Region_pkey");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("Request_pkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("Request_PhysicianId_fkey");

            entity.HasOne(d => d.RequestType).WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_RequestTypeId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("Request_UserId_fkey");
        });

        modelBuilder.Entity<RequestBusiness>(entity =>
        {
            entity.HasKey(e => e.RequestBusinessId).HasName("RequestBusiness_pkey");

            entity.HasOne(d => d.Business).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestBusiness_BusinessId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestBusiness_RequestId_fkey");
        });

        modelBuilder.Entity<RequestClient>(entity =>
        {
            entity.HasKey(e => e.RequestClientId).HasName("RequestClient_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.RequestClients).HasConstraintName("RequestClient_RegionId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestClient_RequestId_fkey");
        });

        modelBuilder.Entity<RequestClosed>(entity =>
        {
            entity.HasKey(e => e.RequestClosedId).HasName("RequestClosed_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestCloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestClosed_RequestId_fkey");

            entity.HasOne(d => d.RequestStatusLog).WithMany(p => p.RequestCloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestClosed_RequestStatusLogId_fkey");
        });

        modelBuilder.Entity<RequestConcierge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RequestConcierge_pkey");

            entity.HasOne(d => d.Concierge).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestConcierge_ConciergeId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestConcierge_RequestId_fkey");
        });

        modelBuilder.Entity<RequestNote>(entity =>
        {
            entity.HasKey(e => e.RequestNotesId).HasName("RequestNotes_pkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestNotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestNotes_RequestId_fkey");
        });

        modelBuilder.Entity<RequestStatusLog>(entity =>
        {
            entity.HasKey(e => e.RequestStatusLogId).HasName("RequestStatusLog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.RequestStatusLogs).HasConstraintName("RequestStatusLog_AdminId_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequestStatusLogPhysicians).HasConstraintName("RequestStatusLog_PhysicianId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestStatusLog_RequestId_fkey");

            entity.HasOne(d => d.TransToPhysician).WithMany(p => p.RequestStatusLogTransToPhysicians).HasConstraintName("RequestStatusLog_TransToPhysicianId_fkey");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("RequestType_pkey");
        });

        modelBuilder.Entity<RequestWiseFile>(entity =>
        {
            entity.HasKey(e => e.RequestWiseFileId).HasName("RequestWiseFile_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.RequestWiseFiles).HasConstraintName("RequestWiseFile_AdminId_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.RequestWiseFiles).HasConstraintName("RequestWiseFile_PhysicianId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestWiseFiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestWiseFile_RequestId_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pkey");
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.RoleMenuId).HasName("RoleMenu_pkey");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoleMenu_MenuId_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoleMenu_RoleId_fkey");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("Shift_pkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Shift_CreatedBy_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Shift_PhysicianId_fkey");
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailId).HasName("ShiftDetail_pkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ShiftDetails).HasConstraintName("ShiftDetail_ModifiedBy_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.ShiftDetails).HasConstraintName("ShiftDetail_RegionId_fkey");

            entity.HasOne(d => d.Shift).WithMany(p => p.ShiftDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ShiftDetail_ShiftId_fkey");
        });

        modelBuilder.Entity<ShiftDetailRegion>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailRegionId).HasName("ShiftDetailRegion_pkey");

            entity.HasOne(d => d.Region).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ShiftDetailRegion_RegionId_fkey");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ShiftDetailRegion_ShiftDetailId_fkey");
        });

        modelBuilder.Entity<Smslog>(entity =>
        {
            entity.HasKey(e => e.SmslogId).HasName("SMSLog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.Smslogs).HasConstraintName("SMSLog_AdminId_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.Smslogs).HasConstraintName("SMSLog_PhysicianId_fkey");

            entity.HasOne(d => d.Request).WithMany(p => p.Smslogs).HasConstraintName("SMSLog_RequestId_fkey");
        });

        modelBuilder.Entity<TimeSheet>(entity =>
        {
            entity.HasKey(e => e.TimeSheetId).HasName("timesheet_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsApproved).HasDefaultValueSql("false");
            entity.Property(e => e.IsFinalize).HasDefaultValueSql("false");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimeSheetCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timesheet_createdby_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimeSheetModifiedByNavigations).HasConstraintName("timesheet_modifiedby_fkey");

            entity.HasOne(d => d.Physician).WithMany(p => p.TimeSheets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timesheet_physicianid_fkey");
        });

        modelBuilder.Entity<TimeSheetDetail>(entity =>
        {
            entity.HasKey(e => e.TimeSheetDetailId).HasName("timesheetdetail_pkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimeSheetDetails).HasConstraintName("timesheetdetail_modifiedby_fkey");

            entity.HasOne(d => d.TimeSheet).WithMany(p => p.TimeSheetDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timesheetdetail_timesheetid_fkey");
        });

        modelBuilder.Entity<TimeSheetDetailReimbursement>(entity =>
        {
            entity.HasKey(e => e.TimeSheetDetailReimbursementId).HasName("timesheetdetailreimbursement_pkey");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("false");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TimeSheetDetailReimbursementCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timesheetdetailreimbursement_createdby_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TimeSheetDetailReimbursementModifiedByNavigations).HasConstraintName("timesheetdetailreimbursement_modifiedby_fkey");

            entity.HasOne(d => d.TimeSheetDetail).WithMany(p => p.TimeSheetDetailReimbursements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timesheetdetailreimbursement_timesheetdetailid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.Users).HasConstraintName("User_AspNetUserId_fkey");

            entity.HasOne(d => d.Region).WithMany(p => p.Users).HasConstraintName("User_RegionId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
