-- 1. Create Update Employee SP
CREATE PROCEDURE sp_UpdateEmployee
    @EmployeeId INT, @Name NVARCHAR(100), @DesignationId INT, @Phone NVARCHAR(20), 
    @ServiceCategory NVARCHAR(100), @BranchId INT, @PhotoPath NVARCHAR(MAX)
AS
BEGIN
    UPDATE Employees
    SET Name = @Name, DesignationId = @DesignationId, Phone = @Phone, 
        ServiceCategory = @ServiceCategory, BranchId = @BranchId, PhotoPath = @PhotoPath
    WHERE EmployeeId = @EmployeeId;
END
GO

-- 2. Create Delete Qualifications SP (Used during edit)
CREATE PROCEDURE sp_DeleteQualificationsByEmployeeId
    @EmployeeId INT
AS
BEGIN
    DELETE FROM EducationalQualifications WHERE EmployeeId = @EmployeeId;
END
GO

-- 3. Create Delete Employee SP
CREATE PROCEDURE sp_DeleteEmployee
    @EmployeeId INT
AS
BEGIN
    -- Delete qualifications first to avoid Foreign Key constraint errors
    DELETE FROM EducationalQualifications WHERE EmployeeId = @EmployeeId;
    
    -- Then delete the employee
    DELETE FROM Employees WHERE EmployeeId = @EmployeeId;
END
GO