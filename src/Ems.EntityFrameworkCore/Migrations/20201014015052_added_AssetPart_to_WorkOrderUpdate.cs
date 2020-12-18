using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class added_AssetPart_to_WorkOrderUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetPartId",
                table: "WorkOrderUpdates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderUpdates_AssetPartId",
                table: "WorkOrderUpdates",
                column: "AssetPartId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderUpdates_AssetParts_AssetPartId",
                table: "WorkOrderUpdates",
                column: "AssetPartId",
                principalTable: "AssetParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderUpdates_AssetParts_AssetPartId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderUpdates_AssetPartId",
                table: "WorkOrderUpdates");

            migrationBuilder.DropColumn(
                name: "AssetPartId",
                table: "WorkOrderUpdates");
        }
    }
}
