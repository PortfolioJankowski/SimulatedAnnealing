using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SimulatedAnnealing.Server.Models;

public partial class PhdApiContext : DbContext
{
    public PhdApiContext()
    {
    }

    public PhdApiContext(DbContextOptions<PhdApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<GerrymanderingResult> GerrymanderingResults { get; set; }

    public virtual DbSet<Neighbour> Neighbours { get; set; }

    public virtual DbSet<Voivodship> Voivodships { get; set; }

    public virtual DbSet<VotingResult> VotingResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-QVSEF8O;Database=PhdApi;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasOne(d => d.Voivodships).WithMany(p => p.Districts)
                .HasForeignKey(d => d.VoivodshipsId)
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

        modelBuilder.Entity<Neighbour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Neighbou__3213E83FF0EB9AB4");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.County).WithMany(p => p.Neighbours)
                .HasForeignKey(d => d.CountyId)
                .HasConstraintName("FK_Sasiedzi_PowiatID");
        });

        modelBuilder.Entity<Voivodship>(entity =>
        {
            entity.HasKey(e => e.VoivodshipsId).HasName("PK__Voivodsh__C28D1B0244656C55");

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
