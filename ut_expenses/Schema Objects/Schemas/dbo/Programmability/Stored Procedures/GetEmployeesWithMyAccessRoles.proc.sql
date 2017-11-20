CREATE PROCEDURE [dbo].[GetEmployeesWithMyAccessRoles]
 @accessRoleIds IntPK READONLY 
AS

SELECT DISTINCT employeeid from dbo.employeeAccessRoles WHERE accessRoleID IN (SELECT c1 FROM @accessRoleIds)

RETURN 0