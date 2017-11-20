CREATE PROCEDURE [IncrementEmployeesCurrentReferenceNumberForExpenseItems]
	@employeeId INT
AS
BEGIN
	UPDATE [employees] SET [currefnum] = [currefnum] + 1 WHERE [employeeid] = @employeeId;
END
