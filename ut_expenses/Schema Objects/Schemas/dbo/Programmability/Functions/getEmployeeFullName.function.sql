CREATE FUNCTION dbo.getEmployeeFullName(@employeeId int) 
RETURNS nvarchar(100)
AS
BEGIN	
	DECLARE @fullname nvarchar(100);

	-- Add the T-SQL statements to compute the return value here
	SELECT @fullname = firstname + ' ' + surname from employees where employeeId = @employeeId

	RETURN @fullname
END
