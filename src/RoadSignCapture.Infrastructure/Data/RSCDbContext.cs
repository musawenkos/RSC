using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Models;

namespace RoadSignCapture.Infrastructure.Data;

public partial class RSCDbContext : DbContext
{
    public RSCDbContext(DbContextOptions<RSCDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971CAC7D32A36F");

            entity.ToTable("Company");

            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.ContactNumber).HasMaxLength(50);
            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.FullAddress).HasMaxLength(500);
            entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A26EDD305");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__User__A9D1053533AFF2B6");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_User_Company");

            entity.HasMany(d => d.Roles).WithMany(p => p.UserEmails)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRole_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserEmail")
                        .HasConstraintName("FK_UserRole_User"),
                    j =>
                    {
                        j.HasKey("UserEmail", "RoleId");
                        j.ToTable("UserRole");
                        j.IndexerProperty<string>("UserEmail").HasMaxLength(256);
                    });

            entity.HasMany(d => d.Projects).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectUser",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectName")
                        .HasConstraintName("FK_ProjectUser_Project")
                        .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserEmail")
                        .HasConstraintName("FK_ProjectUser_User")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("ProjectName", "UserEmail");
                        j.ToTable("ProjectUser");
                        j.IndexerProperty<string>("ProjectName").HasMaxLength(255);
                        j.IndexerProperty<string>("UserEmail").HasMaxLength(256);
                    });

        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectName).HasName("PK_Project");
            entity.ToTable("Project");
            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");
        });

        var seedDate = new DateTime(2025, 01, 01);

        modelBuilder.Entity<Company>().HasData(new Company
        {
            CompanyId = 1,
            CompanyName = "MJNEXUS SYSTEMS",
            FullAddress = "Head Office",
            ContactNumber = "0732347796",
            Created = seedDate,
            Updated = seedDate
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "SysAdmin" },
            new Role { RoleId = 2, RoleName = "Designer" },
            new Role { RoleId = 3, RoleName = "Client" },
            new Role { RoleId = 4, RoleName = "Viewer" }
        );

        modelBuilder.Entity<User>().HasData(new User
        {
            Email = "ndlelamusa1st@gmail.com",
            DisplayName = "M Ndlela",
            CompanyId = 1,
            Created = seedDate,
            Updated = seedDate
        });
        
         modelBuilder.Entity("UserRole").HasData(new
        {
            UserEmail = "ndlelamusa1st@gmail.com",
            RoleId = 1
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
