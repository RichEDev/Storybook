CREATE PROCEDURE [dbo].[APIgetEmployeeAccessRoles]
	@employeeid int
	
AS
	select employeeID, accessRoleID, subAccountID from employeeAccessRoles where employeeid = @employeeid
RETURN 0
