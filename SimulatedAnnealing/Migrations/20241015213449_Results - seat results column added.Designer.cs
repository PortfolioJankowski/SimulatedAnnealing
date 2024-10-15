﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimulatedAnnealing.Models;

#nullable disable

namespace SimulatedAnnealing.Migrations
{
    [DbContext(typeof(SimulatedAnnealingContext))]
    [Migration("20241015213449_Results - seat results column added")]
    partial class Resultsseatresultscolumnadded
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SimulatedAnnealing.Models.GerrymanderingResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ChoosenParty")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<double>("CrackingThreshold")
                        .HasColumnType("float");

                    b.Property<double>("CrackingWeight")
                        .HasColumnType("float");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("ElectoralYear")
                        .HasColumnType("int");

                    b.Property<double>("FinalScore")
                        .HasColumnType("float");

                    b.Property<double>("FinalSeats")
                        .HasColumnType("float");

                    b.Property<double>("InitialScore")
                        .HasColumnType("float");

                    b.Property<double>("InitialSeats")
                        .HasColumnType("float");

                    b.Property<int>("Iterations")
                        .HasColumnType("int");

                    b.Property<string>("JsonData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PackingThreshold")
                        .HasColumnType("float");

                    b.Property<double>("PackingWeight")
                        .HasColumnType("float");

                    b.Property<string>("Results")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ScoreChange")
                        .HasColumnType("float");

                    b.Property<double>("SeatsChange")
                        .HasColumnType("float");

                    b.Property<string>("Voivodeship")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("PK__Gerryman__3214EC07ADBF4BA8");

                    b.ToTable("GerrymanderingResults");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Okregi", b =>
                {
                    b.Property<int>("OkregId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("OkregID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OkregId"));

                    b.Property<int>("Numer")
                        .HasColumnType("int");

                    b.Property<int?>("WojewodztwoId")
                        .HasColumnType("int")
                        .HasColumnName("WojewodztwoID");

                    b.HasKey("OkregId")
                        .HasName("PK__Okregi__3D80556FD736023B");

                    b.HasIndex("WojewodztwoId");

                    b.ToTable("Okregi", "Samorzad");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Powiaty", b =>
                {
                    b.Property<int>("PowiatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PowiatID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PowiatId"));

                    b.Property<int>("LiczbaMieszkancow")
                        .HasColumnType("int");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("OkregId")
                        .HasColumnType("int")
                        .HasColumnName("OkregID");

                    b.HasKey("PowiatId")
                        .HasName("PK__Powiaty__22C9A89D726089F7");

                    b.HasIndex("OkregId");

                    b.ToTable("Powiaty", "Samorzad");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Sasiedzi", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int?>("PowiatId")
                        .HasColumnType("int");

                    b.Property<int?>("SasiadId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__Sasiedzi__3213E83FA7F26E9C");

                    b.HasIndex("PowiatId");

                    b.ToTable("Sasiedzi", (string)null);
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Wojewodztwa", b =>
                {
                    b.Property<int>("WojewodztwoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("WojewodztwoID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WojewodztwoId"));

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("WojewodztwoId")
                        .HasName("PK__Wojewodz__1AFA3316E6EF160B");

                    b.ToTable("Wojewodztwa", (string)null);
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Wyniki", b =>
                {
                    b.Property<int>("WynikiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("WynikiID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WynikiId"));

                    b.Property<string>("Komitet")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("LiczbaGlosow")
                        .HasColumnType("int");

                    b.Property<int?>("PowiatId")
                        .HasColumnType("int")
                        .HasColumnName("PowiatID");

                    b.Property<int>("Rok")
                        .HasColumnType("int");

                    b.HasKey("WynikiId")
                        .HasName("PK__Wyniki__CD0BFEE20D209865");

                    b.HasIndex("PowiatId");

                    b.ToTable("Wyniki", "Samorzad");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Okregi", b =>
                {
                    b.HasOne("SimulatedAnnealing.Models.Wojewodztwa", "Wojewodztwo")
                        .WithMany("Okregis")
                        .HasForeignKey("WojewodztwoId")
                        .HasConstraintName("FK__Okregi__Wojewodz__4316F928");

                    b.Navigation("Wojewodztwo");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Powiaty", b =>
                {
                    b.HasOne("SimulatedAnnealing.Models.Okregi", "Okreg")
                        .WithMany("Powiaties")
                        .HasForeignKey("OkregId")
                        .HasConstraintName("FK__Powiaty__OkregID__45F365D3");

                    b.Navigation("Okreg");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Sasiedzi", b =>
                {
                    b.HasOne("SimulatedAnnealing.Models.Powiaty", "Powiat")
                        .WithMany("Sasiedzis")
                        .HasForeignKey("PowiatId")
                        .HasConstraintName("FK_Sasiedzi_PowiatID");

                    b.Navigation("Powiat");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Wyniki", b =>
                {
                    b.HasOne("SimulatedAnnealing.Models.Powiaty", "Powiat")
                        .WithMany("Wynikis")
                        .HasForeignKey("PowiatId")
                        .HasConstraintName("FK__Wyniki__PowiatID__48CFD27E");

                    b.Navigation("Powiat");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Okregi", b =>
                {
                    b.Navigation("Powiaties");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Powiaty", b =>
                {
                    b.Navigation("Sasiedzis");

                    b.Navigation("Wynikis");
                });

            modelBuilder.Entity("SimulatedAnnealing.Models.Wojewodztwa", b =>
                {
                    b.Navigation("Okregis");
                });
#pragma warning restore 612, 618
        }
    }
}
