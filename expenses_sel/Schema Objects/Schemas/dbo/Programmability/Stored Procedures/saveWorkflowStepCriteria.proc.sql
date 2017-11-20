





CREATE PROCEDURE [dbo].[saveWorkflowStepCriteria]
	@workflowID int,
	@workflowStepID int,
	@fieldID uniqueidentifier,
	@operator int,
	@valueOne nvarchar(150),
	@valueTwo nvarchar(150),
	@runtime bit,
	@updateCriteria bit,
	@replaceElements bit,
@CUemployeeID INT,
@CUdelegateID INT
AS 
	DECLARE @conditionID int;

	INSERT INTO workflowConditions (workflowID, workflowStepID, fieldID, operator, [valueOne], valueTwo, runtime, updateCriteria, replaceElements) VALUES (@workflowID, @workflowStepID, @fieldID, @operator, @valueOne, @valueTwo, @runtime, @updateCriteria, @replaceElements);
	SET @conditionID = scope_identity();

	declare @title nvarchar(500);
	select @title = workflowName from workflows where workflowID=@workflowid;
	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Condition - ' + cast(@workflowStepID as nvarchar(20)) + ' for workflow - ' + @title);

	exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 4, @workflowStepID, @recordTitle, null;
RETURN @conditionID;









