﻿



CREATE PROCEDURE [dbo].[saveWorkflowStepEmailTemplate]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@emailTemplateID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], actionID, showQuestionDialog) VALUES (@workflowid, @parentStepID, @description, 7, @emailTemplateID, 0);

	DECLARE @workflowStepID int;
	SET @workflowStepID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step EmailTemplate - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;








