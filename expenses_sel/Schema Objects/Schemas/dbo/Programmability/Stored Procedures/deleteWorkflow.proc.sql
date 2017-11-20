


CREATE PROCEDURE [dbo].[deleteWorkflow] 
	@workflowID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	declare @recordTitle nvarchar(2000);
	select @recordTitle = workflowName from workflows where workflowID=@workflowID;

	DELETE FROM workflows WHERE workflowID=@workflowID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowID, @recordTitle, null;
		



