using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mechera.Sec.Data.Models;

public partial class MecheraDbContext : DbContext
{
    public MecheraDbContext()
    {
    }

    public MecheraDbContext(DbContextOptions<MecheraDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Username, "username_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsRoot)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("is_root");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(128)
                .HasColumnName("username")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
