using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class Added_MaintenancePlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MaintenancePlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Subject = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    WorkOrderPriorityId = table.Column<int>(nullable: true),
                    WorkOrderTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlans_WorkOrderPriorities_WorkOrderPriorityId",
                        column: x => x.WorkOrderPriorityId,
                        principalTable: "WorkOrderPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenancePlans_WorkOrderTypes_WorkOrderTypeId",
                        column: x => x.WorkOrderTypeId,
                        principalTable: "WorkOrderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlans_TenantId",
                table: "MaintenancePlans",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlans_WorkOrderPriorityId",
                table: "MaintenancePlans",
                column: "WorkOrderPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlans_WorkOrderTypeId",
                table: "MaintenancePlans",
                column: "WorkOrderTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenancePlans");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "WorkOrderUpdates");
        }
    }
}
