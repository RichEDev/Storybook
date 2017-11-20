CREATE PROCEDURE [dbo].[AddSuccessfulApiCallToAuditLog] (
 @employeeId INT, 
 @uri NVARCHAR(MAX),
 @result NVARCHAR(MAX))
AS
BEGIN
 SET NOCOUNT ON;

 DECLARE @username NVARCHAR(50);
 SELECT @username = username FROM employees WHERE employeeid = @employeeid
 INSERT INTO [dbo].[ApiMethodLog] (EmployeeId, UserName, Uri, Result) values (@employeeid, @username, @uri, @result);
END
GO
