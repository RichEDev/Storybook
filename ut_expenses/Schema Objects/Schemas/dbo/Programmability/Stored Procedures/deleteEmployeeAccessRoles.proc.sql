

CREATE PROCEDURE [dbo].[deleteEmployeeAccessRoles]
	@employeeID int
AS 
	BEGIN
		DELETE FROM [dbo].[employeeAccessRoles] WHERE employeeID = @employeeID
	END
