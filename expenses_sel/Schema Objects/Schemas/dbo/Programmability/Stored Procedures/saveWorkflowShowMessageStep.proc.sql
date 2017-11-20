


CREATE PROCEDURE [dbo].[saveWorkflowShowMessageStep]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@message nvarchar(MAX),
	@CUemployeeID INT,
	@CUdelegateID INT
AS 
	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], showQuestionDialog, [message]) VALUES (@workflowid, @parentStepID, @description, 13, 0, @message);

	DECLARE @workflowStepID int;
	SET @workflowStepID = scope_identity();

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;







