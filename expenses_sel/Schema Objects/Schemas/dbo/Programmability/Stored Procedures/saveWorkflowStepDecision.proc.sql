









CREATE PROCEDURE [dbo].[saveWorkflowStepDecision]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@question nvarchar(150),
	@trueOption nvarchar(150),
	@falseOption nvarchar(150),
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @action int;
	SET @action = 4;
	DECLARE @actionID int;
	SET @actionID = NULL;
	DECLARE @showQuestionDialog BIT;
	SET @showQuestionDialog = 1;
	DECLARE @workflowStepID int;


	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], actionID, showQuestionDialog, question, trueOption, falseOption) 
		VALUES (@workflowid, @parentStepID, @description, @action, @actionID, @showQuestionDialog, @question, @trueOption, @falseOption);
	SET @workflowStepID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step Decision - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;








