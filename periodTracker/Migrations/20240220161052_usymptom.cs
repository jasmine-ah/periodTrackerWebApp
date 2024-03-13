using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace periodTracker.Migrations
{
    public partial class usymptom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSymptoms_SymptomTypes_SymptomTypeId",
                table: "UserSymptoms");

            migrationBuilder.DropIndex(
                name: "IX_UserSymptoms_SymptomTypeId",
                table: "UserSymptoms");

            migrationBuilder.DropColumn(
                name: "SymptomTypeId",
                table: "UserSymptoms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SymptomTypeId",
                table: "UserSymptoms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptoms_SymptomTypeId",
                table: "UserSymptoms",
                column: "SymptomTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSymptoms_SymptomTypes_SymptomTypeId",
                table: "UserSymptoms",
                column: "SymptomTypeId",
                principalTable: "SymptomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
