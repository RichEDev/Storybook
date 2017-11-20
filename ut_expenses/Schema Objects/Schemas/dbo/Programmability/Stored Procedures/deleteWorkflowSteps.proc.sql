
CREATE PROCEDURE [dbo].[deleteWorkflowSteps] 
	@workflowid int,
	@employeeID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	UPDATE workflows SET modifiedOn=getdate(), modifiedBy=@employeeID WHERE workflowID=@workflowid
	DELETE FROM workflowConditions WHERE workflowStepID IN (SELECT workflowStepID FROM workflowSteps WHERE workflowID=@workflowid)
	DELETE FROM workflowsApproval WHERE workflowStepID IN (SELECT workflowStepID FROM workflowSteps WHERE workflowID=@workflowid)
	DELETE FROM workflowSteps WHERE workflowID=@workflowid


	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowID;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'All steps for workflow - ' + @title);
	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowID, @recordTitle, null;

