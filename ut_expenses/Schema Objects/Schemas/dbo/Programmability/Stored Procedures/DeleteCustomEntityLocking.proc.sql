CREATE PROCEDURE [dbo].[DeleteCustomEntityLocking]
	@customEntityId int,
	@entityId int
AS
	DELETE FROM CustomEntityLocking WHERE customEntityId = @customEntityId AND entityId = @entityId
RETURN 0
