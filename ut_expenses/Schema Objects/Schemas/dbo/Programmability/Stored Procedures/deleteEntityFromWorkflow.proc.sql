


CREATE PROCEDURE [dbo].[deleteEntityFromWorkflow] (@workflowID int, @entityID int,
@CUemployeeID INT,
@CUdelegateID INT)
AS
begin
	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowID;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'GreenLight - ' + cast(@entityID as nvarchar(20)) + ' in workflow - ' + @title);

	DELETE FROM dbo.workflowEntityState WHERE workflowID=@workflowID AND entityID=@entityID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowID, @recordTitle, null;
end



