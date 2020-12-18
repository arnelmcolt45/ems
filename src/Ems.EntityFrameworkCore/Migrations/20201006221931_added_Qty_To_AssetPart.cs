using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class added_Qty_To_AssetPart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsItem",
                table: "AssetParts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "AssetParts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsItem",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "AssetParts");
        }
    }
}
