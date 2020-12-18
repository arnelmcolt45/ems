using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class customer_terms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportItems_Uoms_UomId",
                table: "SupportItems");

            migrationBuilder.AlterColumn<int>(
                name: "UomId",
                table: "SupportItems",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "PaymentTermNumber",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTermType",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "XeroConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    IsHostConfig = table.Column<bool>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    Failed = table.Column<bool>(nullable: false),
                    ClientSecret = table.Column<string>(nullable: true),
                    CallbackUri = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    AssetOwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XeroConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XeroConfigurations_AssetOwners_AssetOwnerId",
                        column: x => x.AssetOwnerId,
                        principalTable: "AssetOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_XeroConfigurations_AssetOwnerId",
                table: "XeroConfigurations",
                column: "AssetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_XeroConfigurations_TenantId",
                table: "XeroConfigurations",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportItems_Uoms_UomId",
                table: "SupportItems",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportItems_Uoms_UomId",
                table: "SupportItems");

            migrationBuilder.DropTable(
                name: "XeroConfigurations");

            migrationBuilder.DropColumn(
                name: "PaymentTermNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PaymentTermType",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "UomId",
                table: "SupportItems",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportItems_Uoms_UomId",
                table: "SupportItems",
                column: "UomId",
                principalTable: "Uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
