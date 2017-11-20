CREATE FUNCTION dbo.GetTaskOwner(@taskId int)
RETURNS nvarchar(200)
AS
BEGIN
DECLARE @description nvarchar(500);
DECLARE @taskOwnerType int;
DECLARE @taskOwnerId int;
SET @description = '';

SELECT @taskOwnerType = taskOwnerType, @taskOwnerId = taskOwnerId from tasks where taskId = @taskId;

IF @taskOwnerType = 1 -- Team
BEGIN
	SELECT @description = '* ' + teamName FROM teams WHERE teamid = @taskOwnerId;
END
ELSE
BEGIN
	SELECT @description = (firstname + ' ' + surname) FROM employees WHERE employeeid = @taskOwnerId;
END

RETURN @description;
END
