CREATE PROCEDURE [dbo].[APIgetEmployeeRoles]
	@employeeid int 
	
AS
	SELECT employeeid, itemroleid, [order] from employee_roles where employeeid = @employeeid
RETURN 0
