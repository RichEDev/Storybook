CREATE PROCEDURE [dbo].[SMApiDeleteApiDetails]
	@employeeId int
AS
BEGIN
	DELETE FROM [dbo].[ApiDetails] 
	WHERE EmployeeId = @employeeId
	RETURN @@RowCount
END

