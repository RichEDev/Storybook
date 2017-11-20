CREATE PROCEDURE [dbo].[DeleteSignoffStage]
	@signoffId int
AS

	declare @groupId int;
	select @groupId = groupid from signoffs where signoffid = @signoffId;

	delete from signoffs where signoffid = @signoffId;

	exec dbo.UpdateStageNumbers @groupId;

RETURN 0;
