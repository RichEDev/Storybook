










CREATE PROCEDURE [dbo].[SaveWorkflowStepDecisionFalse]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@question nvarchar(150),
	@trueOption nvarchar(150),
	@falseOption nvarchar(150),
	@relatedStep int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @action int;
	SET @action = 12;
	DECLARE @actionID int;
	SET @actionID = NULL;
	DECLARE @showQuestionDialog BIT;
	SET @showQuestionDialog = 1;
	DECLARE @workflowStepID int;

	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], actionID, showQuestionDialog, question, trueOption, falseOption, relatedStepID) VALUES (@workflowid, @parentStepID, @description, @action, @actionID, @showQuestionDialog, @question, @trueOption, @falseOption, @relatedStep);

	SET @workflowStepID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step DecisionFalse - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;









