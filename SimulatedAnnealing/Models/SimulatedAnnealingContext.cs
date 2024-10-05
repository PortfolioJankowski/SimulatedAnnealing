using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimulatedAnnealing.Services.Config;

namespace SimulatedAnnealing.Models;

public partial class SimulatedAnnealingContext : DbContext
{
    public SimulatedAnnealingContext()
    {
        
    }

    public SimulatedAnnealingContext(DbContextOptions<SimulatedAnnealingContext> options)
        : base(options)
    {
         
    }
   
    public virtual DbSet<Okregi> Okregis { get; set; }

    public virtual DbSet<Powiaty> Powiaties { get; set; }

    public virtual DbSet<Sasiedzi> Sasiedzis { get; set; }

    public virtual DbSet<Wojewodztwa> Wojewodztwas { get; set; }

    public virtual DbSet<Wyniki> Wynikis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer(Configuration.Config.GetConnectionString("SimmulatedAnnealing"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Okregi>(entity =>
        {
            entity.HasKey(e => e.OkregId).HasName("PK__Okregi__3D80556FD736023B");

            entity.ToTable("Okregi", "Samorzad");

            entity.Property(e => e.OkregId).HasColumnName("OkregID");
            entity.Property(e => e.WojewodztwoId).HasColumnName("WojewodztwoID");

            entity.HasOne(d => d.Wojewodztwo).WithMany(p => p.Okregis)
                .HasForeignKey(d => d.WojewodztwoId)
                .HasConstraintName("FK__Okregi__Wojewodz__4316F928");
        });

        modelBuilder.Entity<Powiaty>(entity =>
        {
            entity.HasKey(e => e.PowiatId).HasName("PK__Powiaty__22C9A89D726089F7");

            entity.ToTable("Powiaty", "Samorzad");

            entity.Property(e => e.PowiatId).HasColumnName("PowiatID");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OkregId).HasColumnName("OkregID");

            entity.HasOne(d => d.Okreg).WithMany(p => p.Powiaties)
                .HasForeignKey(d => d.OkregId)
                .HasConstraintName("FK__Powiaty__OkregID__45F365D3");
        });

        modelBuilder.Entity<Sasiedzi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sasiedzi__3213E83FA7F26E9C");

            entity.ToTable("Sasiedzi");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.Powiat).WithMany(p => p.Sasiedzis)
                .HasForeignKey(d => d.PowiatId)
                .HasConstraintName("FK_Sasiedzi_PowiatID");
        });

        modelBuilder.Entity<Wojewodztwa>(entity =>
        {
            entity.HasKey(e => e.WojewodztwoId).HasName("PK__Wojewodz__1AFA3316E6EF160B");

            entity.ToTable("Wojewodztwa");

            entity.Property(e => e.WojewodztwoId).HasColumnName("WojewodztwoID");
            entity.Property(e => e.Nazwa)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Wyniki>(entity =>
        {
            entity.HasKey(e => e.WynikiId).HasName("PK__Wyniki__CD0BFEE20D209865");

            entity.ToTable("Wyniki", "Samorzad");

            entity.Property(e => e.WynikiId).HasColumnName("WynikiID");
            entity.Property(e => e.Komitet)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PowiatId).HasColumnName("PowiatID");

            entity.HasOne(d => d.Powiat).WithMany(p => p.Wynikis)
                .HasForeignKey(d => d.PowiatId)
                .HasConstraintName("FK__Wyniki__PowiatID__48CFD27E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
