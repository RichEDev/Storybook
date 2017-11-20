







CREATE PROCEDURE [dbo].[saveWorkflowStepFinishWorkflow]
	@workflowid int,
	@parentStepID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @action int;
	SET @action = 9;
	DECLARE @actionID int;
	SET @actionID = NULL;
	DECLARE @showQuestionDialog BIT;
	SET @showQuestionDialog = 1;
	DECLARE @workflowStepID int;

	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], showQuestionDialog) 
		VALUES (@workflowid, @parentStepID, 'Finish Workflow', @action, 0);
	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;

	SET @workflowStepID =  scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step FinishWorkflow - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

RETURN @workflowStepID;






