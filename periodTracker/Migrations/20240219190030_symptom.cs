using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace periodTracker.Migrations
{
    public partial class symptom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Severities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Severities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SymptomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subtypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeverityId = table.Column<int>(type: "int", nullable: true),
                    SymptomTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtypes_Severities_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "Severities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subtypes_SymptomTypes_SymptomTypeId",
                        column: x => x.SymptomTypeId,
                        principalTable: "SymptomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSymptoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymptomTypeId = table.Column<int>(type: "int", nullable: false),
                    SubTypeId = table.Column<int>(type: "int", nullable: false),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSymptoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSymptoms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSymptoms_Severities_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "Severities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSymptoms_Subtypes_SubTypeId",
                        column: x => x.SubTypeId,
                        principalTable: "Subtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSymptoms_SymptomTypes_SymptomTypeId",
                        column: x => x.SymptomTypeId,
                        principalTable: "SymptomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subtypes_SeverityId",
                table: "Subtypes",
                column: "SeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtypes_SymptomTypeId",
                table: "Subtypes",
                column: "SymptomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptoms_SeverityId",
                table: "UserSymptoms",
                column: "SeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptoms_SubTypeId",
                table: "UserSymptoms",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptoms_SymptomTypeId",
                table: "UserSymptoms",
                column: "SymptomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptoms_UserId",
                table: "UserSymptoms",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSymptoms");

            migrationBuilder.DropTable(
                name: "Subtypes");

            migrationBuilder.DropTable(
                name: "Severities");

            migrationBuilder.DropTable(
                name: "SymptomTypes");
        }
    }
}
