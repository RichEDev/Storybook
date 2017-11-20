CREATE PROCEDURE [dbo].[GetCustomEntityLocking]
	@customEntityId int,
	@entityId int
AS
	
	SELECT LockedBy, lockedDateTime FROM CustomEntityLocking WHERE customEntityId = @customEntityId AND entityId = @entityId AND lockeddatetime >  DATEADD(MINUTE,-1,GETUTCDATE())
