CREATE PROCEDURE [dbo].[GetWorkflowById]
 @WorkflowID INT
AS
SET NOCOUNT ON
BEGIN
	SELECT WorkflowName,[Description], CreatedOn, CreatedBy, WorkflowType, CanBeChildWorkflow, RunOnCreation, RunOnChange, RunOnDelete, BaseTableID FROM Workflows Where WorkflowID = @WorkflowID
END
