CREATE PROCEDURE [dbo].[UpdateCustomEntityLocking]
	@customEntityId int,
	@entityId int,
	@employeeId int
AS
	UPDATE CustomEntityLocking SET LockedDateTime = GETUTCDATE() WHERE customEntityId = @customEntityId AND entityId = @entityId AND lockedBy = @employeeId
RETURN 0