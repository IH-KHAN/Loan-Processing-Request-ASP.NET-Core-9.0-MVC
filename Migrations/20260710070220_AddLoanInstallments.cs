using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loan_Processing_Inzamam.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanInstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanInstallments",
                columns: table => new
                {
                    InstallmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanRequestId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInstallments", x => x.InstallmentId);
                    table.ForeignKey(
                        name: "FK_LoanInstallments_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_LoanInstallments_LoanRequests_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "LoanRequests",
                        principalColumn: "LoanRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_BankAccountId",
                table: "LoanInstallments",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_LoanRequestId",
                table: "LoanInstallments",
                column: "LoanRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanInstallments");
        }
    }
}
