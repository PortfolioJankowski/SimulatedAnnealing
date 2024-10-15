using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Migrations
{
    /// <inheritdoc />
    public partial class resultstableexpand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<double>(
                       name: "PackingThreshold",
                       table: "GerrymanderingResults",
                       nullable: false,
                       defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CrackingThreshold",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PackingWeight",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CrackingWeight",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "InitialScore",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinalScore",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ScoreChange",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "InitialSeats",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinalSeats",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SeatsChange",
                table: "GerrymanderingResults",
                nullable: false,
                defaultValue: 0.0);
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PackingThreshold", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "CrackingThreshold", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "PackingWeight", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "CrackingWeight", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "InitialScore", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "FinalScore", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "ScoreChange", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "InitialSeats", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "FinalSeats", table: "GerrymanderingResults");
            migrationBuilder.DropColumn(name: "SeatsChange", table: "GerrymanderingResults");
        }
    }
}
