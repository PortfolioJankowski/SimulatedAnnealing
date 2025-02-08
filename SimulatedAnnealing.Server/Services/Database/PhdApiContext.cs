using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Authentication;

namespace SimulatedAnnealing.Server.Services.Database;

public partial class PhdApiContext : IdentityDbContext<AppUser>
{
    public PhdApiContext(DbContextOptions<PhdApiContext> options)
        : base(options)
    {

    }
    private readonly Dictionary<string, string> _roleNames;
    public virtual DbSet<County> Counties { get; set; }
    public virtual DbSet<District> Districts { get; set; }
    public virtual DbSet<GerrymanderingResult> GerrymanderingResults { get; set; }
    public virtual DbSet<Neighbor> Neighbors { get; set; }
    public virtual DbSet<Voivodeship> Voivodeships { get; set; }
    public virtual DbSet<VotingResult> VotingResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        base.OnModelCreating(modelBuilder);
        var roles = RoleOptions.RoleNames
            .Keys
            .Select(key => new IdentityRole
            {
                Name = key.ToString(),
                NormalizedName = RoleOptions.RoleNames[key]
            });
        
        modelBuilder.Entity<IdentityRole>().HasData(roles);

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.CountyId).HasName("PK__Counties__B68F9D973EADF06B");

            entity.ToTable("Counties", "LocalGovernment");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.District).WithMany(p => p.Counties)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK__Counties__Distri__5070F446");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__District__85FDA4C6BF4952B8");

            entity.ToTable("Districts", "LocalGovernment");

            entity.HasOne(d => d.Voivodeship).WithMany(p => p.Districts)
                .HasForeignKey(d => d.VoivodeshipsId)
                .HasConstraintName("FK__Districts__Voivo__4F7CD00D");
        });

        modelBuilder.Entity<GerrymanderingResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gerryman__3214EC07D3AD156B");

            entity.Property(e => e.ChoosenParty)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CrackingThreshold).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.CrackingWeight).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FinalScore).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.FinalSeats).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.InitialScore).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.InitialSeats).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.PackingThreshold).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.PackingWeight).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.Results).HasDefaultValue("");
            entity.Property(e => e.ScoreChange).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.SeatsChange).HasDefaultValueSql("((0.0000000000000000e+000))");
            entity.Property(e => e.Voivodeship).HasDefaultValue("");
        });

        modelBuilder.Entity<Neighbor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Neighbou__3213E83FF0EB9AB4");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.County).WithMany(p => p.Neighbors)
                .HasForeignKey(d => d.CountyId)
                .HasConstraintName("FK_Sasiedzi_PowiatID");
        });

        modelBuilder.Entity<Voivodeship>(entity =>
        {
            entity.HasKey(e => e.VoivodeshipsId).HasName("PK__Voivodsh__C28D1B0244656C55");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VotingResult>(entity =>
        {
            entity.HasKey(e => e.ResultsId).HasName("PK__Wyniki__CD0BFEE20D209865");

            entity.ToTable("VotingResults", "LocalGovernment");

            entity.Property(e => e.Committee)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.County).WithMany(p => p.VotingResults)
                .HasForeignKey(d => d.CountyId)
                .HasConstraintName("FK__Wyniki__PowiatID__48CFD27E");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
