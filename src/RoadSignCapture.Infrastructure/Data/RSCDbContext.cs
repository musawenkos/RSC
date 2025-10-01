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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
