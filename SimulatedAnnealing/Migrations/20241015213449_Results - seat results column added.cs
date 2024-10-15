using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Migrations
{
    /// <inheritdoc />
    public partial class Resultsseatresultscolumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Results",
                table: "GerrymanderingResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Results",
                table: "GerrymanderingResults");
        }
    }
}
