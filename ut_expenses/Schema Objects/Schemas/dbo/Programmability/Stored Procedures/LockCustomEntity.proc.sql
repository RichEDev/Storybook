CREATE PROCEDURE [dbo].[LockCustomEntity]
	@customEntityId int,
	@entityId int,
	@employeeId int
AS
	DECLARE @result INT = 0;
	DECLARE @currentDate DateTime = GETUTCDATE();
	DELETE FROM CustomEntityLocking WHERE customEntityId = @customEntityId AND entityId = @entityId AND lockeddatetime <  DATEADD(MINUTE,-1,@currentDate)
	
	IF NOT EXISTS(SELECT LockedBy, lockedDateTime FROM CustomEntityLocking WHERE customEntityId = @customEntityId AND entityId = @entityId AND lockeddatetime >  DATEADD(MINUTE,-1,@currentDate))
	BEGIN
		INSERT INTO CustomEntityLocking (customEntityId, entityId, lockedBy, LockedDateTime) VALUES (@customEntityId, @entityId, @employeeId, @currentDate)		
		SET @result = 1;
	END


RETURN @result;
