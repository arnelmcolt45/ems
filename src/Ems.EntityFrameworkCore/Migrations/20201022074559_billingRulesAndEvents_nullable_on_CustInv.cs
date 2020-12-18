using Microsoft.EntityFrameworkCore.Migrations;

namespace Ems.Migrations
{
    public partial class billingRulesAndEvents_nullable_on_CustInv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_BillingEvents_BillingEventId",
                table: "CustomerInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_BillingRules_BillingRuleId",
                table: "CustomerInvoices");

            migrationBuilder.AlterColumn<int>(
                name: "BillingRuleId",
                table: "CustomerInvoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BillingEventId",
                table: "CustomerInvoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_BillingEvents_BillingEventId",
                table: "CustomerInvoices",
                column: "BillingEventId",
                principalTable: "BillingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_BillingRules_BillingRuleId",
                table: "CustomerInvoices",
                column: "BillingRuleId",
                principalTable: "BillingRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_BillingEvents_BillingEventId",
                table: "CustomerInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerInvoices_BillingRules_BillingRuleId",
                table: "CustomerInvoices");

            migrationBuilder.AlterColumn<int>(
                name: "BillingRuleId",
                table: "CustomerInvoices",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BillingEventId",
                table: "CustomerInvoices",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_BillingEvents_BillingEventId",
                table: "CustomerInvoices",
                column: "BillingEventId",
                principalTable: "BillingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerInvoices_BillingRules_BillingRuleId",
                table: "CustomerInvoices",
                column: "BillingRuleId",
                principalTable: "BillingRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
