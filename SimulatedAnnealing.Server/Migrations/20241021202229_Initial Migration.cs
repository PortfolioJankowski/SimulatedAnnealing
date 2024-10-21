using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimulatedAnnealing.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LocalGovernment");

            migrationBuilder.CreateTable(
                name: "GerrymanderingResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iterations = table.Column<int>(type: "int", nullable: false),
                    ChoosenParty = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    ElectoralYear = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PackingThreshold = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    CrackingThreshold = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    PackingWeight = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    CrackingWeight = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    InitialScore = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    FinalScore = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    ScoreChange = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    InitialSeats = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    FinalSeats = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    SeatsChange = table.Column<double>(type: "float", nullable: false, defaultValueSql: "((0.0000000000000000e+000))"),
                    Voivodeship = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Gerryman__3214EC07D3AD156B", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Voivodeships",
                columns: table => new
                {
                    VoivodeshipsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Voivodsh__C28D1B0244656C55", x => x.VoivodeshipsId);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                schema: "LocalGovernment",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    VoivodeshipsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__District__85FDA4C6BF4952B8", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK__Districts__Voivo__4F7CD00D",
                        column: x => x.VoivodeshipsId,
                        principalTable: "Voivodeships",
                        principalColumn: "VoivodeshipsId");
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                schema: "LocalGovernment",
                columns: table => new
                {
                    CountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    Inahabitants = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Counties__B68F9D973EADF06B", x => x.CountyId);
                    table.ForeignKey(
                        name: "FK__Counties__Distri__5070F446",
                        column: x => x.DistrictId,
                        principalSchema: "LocalGovernment",
                        principalTable: "Districts",
                        principalColumn: "DistrictId");
                });

            migrationBuilder.CreateTable(
                name: "Neighbours",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    CountyId = table.Column<int>(type: "int", nullable: true),
                    NeighborId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Neighbou__3213E83FF0EB9AB4", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sasiedzi_PowiatID",
                        column: x => x.CountyId,
                        principalSchema: "LocalGovernment",
                        principalTable: "Counties",
                        principalColumn: "CountyId");
                });

            migrationBuilder.CreateTable(
                name: "VotingResults",
                schema: "LocalGovernment",
                columns: table => new
                {
                    ResultsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CountyId = table.Column<int>(type: "int", nullable: true),
                    Committee = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    NumberVotes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Wyniki__CD0BFEE20D209865", x => x.ResultsId);
                    table.ForeignKey(
                        name: "FK__Wyniki__PowiatID__48CFD27E",
                        column: x => x.CountyId,
                        principalSchema: "LocalGovernment",
                        principalTable: "Counties",
                        principalColumn: "CountyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counties_DistrictId",
                schema: "LocalGovernment",
                table: "Counties",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_VoivodeshipsId",
                schema: "LocalGovernment",
                table: "Districts",
                column: "VoivodeshipsId");

            migrationBuilder.CreateIndex(
                name: "IX_Neighbours_CountyId",
                table: "Neighbours",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingResults_CountyId",
                schema: "LocalGovernment",
                table: "VotingResults",
                column: "CountyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GerrymanderingResults");

            migrationBuilder.DropTable(
                name: "Neighbours");

            migrationBuilder.DropTable(
                name: "VotingResults",
                schema: "LocalGovernment");

            migrationBuilder.DropTable(
                name: "Counties",
                schema: "LocalGovernment");

            migrationBuilder.DropTable(
                name: "Districts",
                schema: "LocalGovernment");

            migrationBuilder.DropTable(
                name: "Voivodeships");
        }
    }
}
