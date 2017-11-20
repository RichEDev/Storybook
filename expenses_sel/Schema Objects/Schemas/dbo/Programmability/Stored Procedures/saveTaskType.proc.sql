CREATE PROCEDURE [dbo].[saveTaskType] 
(
@ID int, 
@subAccountId int, 
@typeDescription nvarchar(100),
@employeeId int,
@delegateID int
)
AS
DECLARE @count INT;
DECLARE @retVal INT;
DECLARE @recordTitle nvarchar(2000);

IF @ID = -1
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_tasktypes WHERE typeDescription = @typeDescription AND subAccountId = @subAccountId);
	
	IF @count = 0
	BEGIN
		INSERT INTO codes_tasktypes (subAccountId, typeDescription, archived, createdOn, createdBy)
		VALUES (@subAccountId, @typeDescription, 0, getutcdate(), @employeeId);
		
		SET @retVal = scope_identity();
		
		set @recordTitle = 'Task Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @typeDescription + ')';
		exec addInsertEntryToAuditLog @employeeId, @delegateID, 120, @retVal, @recordTitle, @subAccountId;
	END
END
ELSE
BEGIN
	SET @count = (SELECT COUNT(*) FROM codes_tasktypes WHERE typeId <> @ID AND subAccountId = @subAccountId AND typeDescription = @typeDescription);
	
	IF @count = 0
	BEGIN
		DECLARE @oldtypeDescription nvarchar(100);
		
		select @oldtypeDescription = typeDescription from codes_tasktypes where typeId = @ID;
		
		UPDATE codes_tasktypes SET typeDescription = @typeDescription, modifiedOn = getutcdate(), modifiedBy = @employeeId WHERE typeId = @ID;
		
		SET @retVal = @ID;
		
		set @recordTitle = 'Task Type ID: ' + CAST(@retVal AS nvarchar(20)) + ' (' + @typeDescription + ')';

		if @oldtypeDescription <> @typeDescription
			exec addUpdateEntryToAuditLog @employeeId, @delegateID, 120, @ID, '4BCC7AA0-83D3-4AB9-9A61-6B72AE841E3A', @oldtypeDescription, @typeDescription, @recordtitle, @subAccountId;
	END
END

RETURN @retVal;

