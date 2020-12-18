using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_AssetOwnerToInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetOwnerId",
                table: "CustomerInvoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInvoices_AssetOwnerId",
                table: "CustomerInvoices",
                column: "AssetOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_AssetOwners_AssetOwnerId",
                table: "CustomerInvoices",
                column: "AssetOwnerId",
                principalTable: "AssetOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_AssetOwners_AssetOwnerId",
                table: "CustomerInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CustomerInvoices_AssetOwnerId",
                table: "CustomerInvoices");

            migrationBuilder.DropColumn(
                name: "AssetOwnerId",
                table: "CustomerInvoices");
        }
    }
}
