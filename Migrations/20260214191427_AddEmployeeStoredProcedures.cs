using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loan_Processing_Inzamam.Migrations
{
    public partial class AddEmployeeStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. SP for Inserting Employee
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_InsertEmployee
                    @ApplicationUserId NVARCHAR(450), @Name NVARCHAR(100), @DesignationId INT, 
                    @Phone NVARCHAR(20), @DateOfBirth DATETIME2, @PhotoPath NVARCHAR(MAX), 
                    @MaritalStatus NVARCHAR(20), @PresentAddress NVARCHAR(250), @PermanentAddress NVARCHAR(250), 
                    @IsSameAddress BIT, @NIDNumber NVARCHAR(17), @IsActive BIT, 
                    @ServiceCategory NVARCHAR(100), @BranchId INT, @CreatedAt DATETIME2,
                    @NewEmployeeId INT OUTPUT
                AS
                BEGIN
                    INSERT INTO Employees (ApplicationUserId, Name, DesignationId, Phone, DateOfBirth, PhotoPath, MaritalStatus, PresentAddress, PermanentAddress, IsSameAddress, NIDNumber, IsActive, ServiceCategory, BranchId, CreatedAt)
                    VALUES (@ApplicationUserId, @Name, @DesignationId, @Phone, @DateOfBirth, @PhotoPath, @MaritalStatus, @PresentAddress, @PermanentAddress, @IsSameAddress, @NIDNumber, @IsActive, @ServiceCategory, @BranchId, @CreatedAt);
                    
                    SET @NewEmployeeId = SCOPE_IDENTITY();
                END
            ");

            // 2. SP for Inserting Qualifications
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_InsertQualification
                    @DegreeName NVARCHAR(100), @InstitutionName NVARCHAR(150), 
                    @PassingYear INT, @GradingScore FLOAT, @OutOf FLOAT, @EmployeeId INT
                AS
                BEGIN
                    INSERT INTO EducationalQualifications (DegreeName, InstitutionName, PassingYear, GradingScore, OutOf, EmployeeId)
                    VALUES (@DegreeName, @InstitutionName, @PassingYear, @GradingScore, @OutOf, @EmployeeId);
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_InsertEmployee");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_InsertQualification");
        }
    }
}