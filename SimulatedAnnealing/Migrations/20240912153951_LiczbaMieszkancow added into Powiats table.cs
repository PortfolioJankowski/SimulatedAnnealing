using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Migrations
{
    /// <inheritdoc />
    public partial class LiczbaMieszkancowaddedintoPowiatstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LiczbaMieszkancow",
                schema: "Samorzad",
                table: "Powiaty",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiczbaMieszkancow",
                schema: "Samorzad",
                table: "Powiaty");
        }
    }
}
