CREATE PROCEDURE [dbo].[saveWorkflowStepApproval]
	@workflowid int,
	@parentStepID int,
	@relatedStepID int,
	@description nvarchar(4000),
	@approverType int, 
	@approverID int,
	@useOneClickSignoff bit,
	@showQuestionDialog bit,
	@filterItems bit,
	@question nvarchar(4000),
	@trueResponse nvarchar(4000),
	@falseResponse nvarchar(4000),
	@emailTemplateID int,
	@message nvarchar(MAX),
@CUemployeeID INT,
@CUdelegateID INT
AS
DECLARE @action int;
	SET @action = 1;
	DECLARE @actionID int;
	SET @actionID = NULL;

	DECLARE @workflowStepID int;


	INSERT INTO workflowSteps (workflowID, parentStepID, relatedStepID, description, [action], actionID, showQuestionDialog, question, trueOption, falseOption, [message]) VALUES (@workflowid, @parentStepID, @relatedStepID, @description, @action, @actionID, @showQuestionDialog, @question, @trueResponse, @falseResponse, @message);
	SET @workflowStepID = scope_identity();


	INSERT INTO workflowsApproval (workflowStepID, workflowID, approverType, approverID, oneClickSignOff, filterItems, emailTemplateID) VALUES (@workflowStepID, @workflowid, @approverType, @approverID, @useOneClickSignoff, @filterItems, @emailTemplateID);

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step Approval - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;
