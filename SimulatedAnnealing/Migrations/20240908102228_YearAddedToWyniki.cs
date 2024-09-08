using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Migrations
{
    /// <inheritdoc />
    public partial class YearAddedToWyniki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Samorzad");

            migrationBuilder.DropTable(
                name: "Wyniki",
                schema: "Samorzad");

            migrationBuilder.CreateTable(
                name: "Wyniki",
                schema: "Samorzad",
                columns: table => new
                {
                    WynikiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rok = table.Column<int>(type: "int", nullable: false),
                    PowiatID = table.Column<int>(type: "int", nullable: true),
                    Komitet = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    LiczbaGlosow = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Wyniki__CD0BFEE20D209865", x => x.WynikiID);
                    table.ForeignKey(
                        name: "FK__Wyniki__PowiatID__48CFD27E",
                        column: x => x.PowiatID,
                        principalSchema: "Samorzad",
                        principalTable: "Powiaty",
                        principalColumn: "PowiatID");
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wyniki",
                schema: "Samorzad");

            migrationBuilder.CreateTable(
            name: "Wyniki",
            schema: "Samorzad",
            columns: table => new
            {
                WynikiID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PowiatID = table.Column<int>(type: "int", nullable: true),
                Komitet = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                LiczbaGlosow = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Wyniki__CD0BFEE20D209865", x => x.WynikiID);
                table.ForeignKey(
                    name: "FK__Wyniki__PowiatID__48CFD27E",
                    column: x => x.PowiatID,
                    principalSchema: "Samorzad",
                    principalTable: "Powiaty",
                    principalColumn: "PowiatID");
            });

        }
    }
}
