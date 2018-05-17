CREATE FUNCTION [dbo].[GetEmployeeUserNameById] (@EmployeeId int)
RETURNS nvarchar(255)
AS
BEGIN

  DECLARE @UserName nvarchar(255);

  IF @EmployeeId IS NULL
  BEGIN
    RETURN 'System'
  END

  SELECT
    @UserName = username
  FROM employees
  WHERE employeeid = @EmployeeId
  RETURN @UserName;

END
GO