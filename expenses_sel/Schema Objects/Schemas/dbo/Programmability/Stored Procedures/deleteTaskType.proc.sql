CREATE PROCEDURE [dbo].[deleteTaskType]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @typeDescription nvarchar(100);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @typeDescription = typeDescription, @subAccountId = subAccountId from codes_tasktypes where typeId = @ID;

select @cnt = COUNT(taskId) from tasks where taskTypeId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END
	
delete from codes_tasktypes where typeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 120, @ID, @typeDescription, @subAccountId;

return @cnt

