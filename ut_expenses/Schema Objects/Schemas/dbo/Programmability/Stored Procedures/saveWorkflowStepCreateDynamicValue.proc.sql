CREATE PROCEDURE [dbo].[saveWorkflowStepCreateDynamicValue]
	@workflowStepID int,
	@dynamicValueFormula nvarchar(max),
	@fieldID uniqueidentifier,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @dynamicValueID int;

	INSERT INTO workflowDynamicValues (workflowStepID, dynamicValueFormula, fieldID) VALUES (@workflowStepID, @dynamicValueFormula, @fieldID);
	SET @dynamicValueID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=(select workflowid from workflowSteps where workflowStepID = @workflowStepID);
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Dynamic Value - ' + cast(@dynamicValueID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @dynamicValueID, @recordTitle, null;
RETURN @dynamicValueID;
