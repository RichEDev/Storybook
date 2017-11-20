




CREATE PROCEDURE [dbo].[saveWorkflowStepCheckCondition]
	@workflowid int,
	@parentStepID int,
	@description nvarchar(4000),
	@stepAction int,
	@relatedStepID int,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	INSERT INTO workflowSteps (workflowID, parentStepID, description, [action], relatedStepID) VALUES (@workflowid, @parentStepID, @description, @stepAction, @relatedStepID);

	DECLARE @workflowStepID int;
	SET @workflowStepID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowID;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Step CheckCondition- ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;

	UPDATE workflows SET modifiedon=getdate() WHERE workflowID=@workflowid;
RETURN @workflowStepID;









