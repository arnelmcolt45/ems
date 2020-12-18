using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class added_WarehouseID_to_AssetPart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_WarehouseId",
                table: "AssetParts",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_Warehouses_WarehouseId",
                table: "AssetParts",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_Warehouses_WarehouseId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_WarehouseId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "AssetParts");
        }
    }
}
