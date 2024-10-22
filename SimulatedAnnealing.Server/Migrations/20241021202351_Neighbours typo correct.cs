using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Server.Migrations
{
    /// <inheritdoc />
    public partial class Neighbourstypocorrect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameTable(
            //    name: "Neighbours",
            //    newName: "Neighbors");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Neighbours_CountyId",
            //    table: "Neighbors",
            //    newName: "IX_Neighbors_CountyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Neighbors",
                newName: "Neighbours");

            migrationBuilder.RenameIndex(
                name: "IX_Neighbors_CountyId",
                table: "Neighbours",
                newName: "IX_Neighbours_CountyId");
        }
    }
}
