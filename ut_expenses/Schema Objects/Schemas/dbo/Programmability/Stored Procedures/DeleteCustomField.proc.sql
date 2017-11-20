CREATE PROCEDURE [dbo].[DeleteCustomField] 
	@FieldID uniqueidentifier
AS

DECLARE @WorkflowConditionCount int;
DECLARE @CustEntViewCount int;
DECLARE @FieldCat tinyint;

BEGIN
	SELECT @FieldCat = FieldCategory FROM [customFields] WHERE fieldid = @FieldID;
	SET @WorkflowConditionCount = (SELECT count(conditionID) FROM workflowConditions WHERE fieldID = @FieldID);
	IF @WorkflowConditionCount > 0
	BEGIN
		RETURN -6; --Assigned to a workflow condition
	END
	
	if @FieldCat <> 3 --If field type is AliasTableField then this check should be ignored 
	BEGIN
		SET @CustEntViewCount = (SELECT count(viewID) FROM [customEntityViewFields] WHERE fieldid = @FieldID);
		IF @CustEntViewCount > 0
		BEGIN
			RETURN -7; --Assigned to a custom entity view
		END
	END		

	DELETE FROM [customFields] WHERE fieldid = @FieldID;
	RETURN 0;
END
