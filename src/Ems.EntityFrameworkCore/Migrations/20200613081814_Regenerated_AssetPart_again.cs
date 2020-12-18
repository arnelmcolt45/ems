using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Regenerated_AssetPart_again : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_Attachments_AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetParts_UsageMetrics_UsageMetricId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropIndex(
                name: "IX_AssetParts_UsageMetricId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "AssetParts");

            migrationBuilder.DropColumn(
                name: "UsageMetricId",
                table: "AssetParts");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetParts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "AssetParts",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "AttachmentId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageMetricId",
                table: "AssetParts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_AttachmentId",
                table: "AssetParts",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParts_UsageMetricId",
                table: "AssetParts",
                column: "UsageMetricId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_Attachments_AttachmentId",
                table: "AssetParts",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParts_UsageMetrics_UsageMetricId",
                table: "AssetParts",
                column: "UsageMetricId",
                principalTable: "UsageMetrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
