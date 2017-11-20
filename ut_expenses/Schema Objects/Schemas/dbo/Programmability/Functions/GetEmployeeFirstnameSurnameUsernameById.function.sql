CREATE FUNCTION [dbo].[GetEmployeeFirstnameSurnameUsernameById] (@employeeId INT)
RETURNS NVARCHAR(354)
AS
BEGIN
	DECLARE @name nvarchar(354);
	SELECT @name = [firstname] + ' ' + [surname] + ' (' + [username] + ')' FROM [employees] WHERE [employeeId] = @employeeId;

	RETURN @name;
END