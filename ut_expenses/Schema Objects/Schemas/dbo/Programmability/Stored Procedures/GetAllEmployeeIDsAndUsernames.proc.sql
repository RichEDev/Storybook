CREATE PROCEDURE [dbo].[GetAllEmployeeIDsAndUsernames]
	
AS
BEGIN
	SELECT employeeid, username FROM dbo.employees WHERE username NOT LIKE '%admin%'
END
