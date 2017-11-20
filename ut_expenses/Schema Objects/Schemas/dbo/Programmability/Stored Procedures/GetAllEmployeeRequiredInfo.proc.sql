CREATE PROCEDURE [dbo].[GetAllEmployeeRequiredInfo]
	
AS
BEGIN
	SELECT employeeid, username, title, firstname, surname FROM dbo.employees WHERE username NOT LIKE '%admin%'
END
