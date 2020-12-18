using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class added_Completed_to_WorkOrderUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "WorkOrderUpdates",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "WorkOrderUpdates");
        }
    }
}
