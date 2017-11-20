IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_NAME = 'GetAllEmployeeRequiredInfo')
BEGIN
DROP PROCEDURE [dbo].[GetAllEmployeeRequiredInfo]
END
GO

CREATE PROCEDURE [dbo].[GetAllEmployeeRequiredInfo]
AS
BEGIN
 SELECT employeeid, username, title, firstname, surname FROM dbo.employees WHERE username NOT LIKE 'admin%'
END
GO