CREATE FUNCTION dbo.GetEmployeeNameById(@employeeId int)
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @name nvarchar(100);
	SELECT @name = firstname + ' ' + surname from employees where employeeId = @employeeId;

	RETURN @name;
END
