



CREATE PROCEDURE [dbo].[saveWorkflowStepRunSubWorkflow] 
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@actionID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @action int;
	SET @action = 6;

	DECLARE @trueOption nvarchar(150);
	SET @trueOption = NULL;
	DECLARE @falseOption nvarchar(150);
	SET @falseOption = NULL;
	DECLARE @question nvarchar(150);
	SET @question = NULL;
	DECLARE @showQuestionDialog BIT;
	SET @showQuestionDialog = 0;

	DECLARE @workflowStepID int;

	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], actionID, showQuestionDialog, question, trueOption, falseOption) VALUES (@workflowid, @parentStepID, @description, @action, @actionID, @showQuestionDialog, @question, @trueOption, @falseOption);
	SET @workflowStepID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step RunSubWorkflow - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;

	RETURN @workflowStepID;



