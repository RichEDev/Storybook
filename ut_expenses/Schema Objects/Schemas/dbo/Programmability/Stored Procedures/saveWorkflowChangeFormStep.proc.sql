



CREATE PROCEDURE [dbo].[saveWorkflowChangeFormStep]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@formID int,
	@CUemployeeID INT,
	@CUdelegateID INT
AS 
	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], showQuestionDialog, formID) VALUES (@workflowid, @parentStepID, @description, 14, 0, @formID);

	DECLARE @workflowStepID int;
	SET @workflowStepID = scope_identity();

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;








