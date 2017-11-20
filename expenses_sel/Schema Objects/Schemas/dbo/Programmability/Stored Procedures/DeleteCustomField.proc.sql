CREATE PROCEDURE [dbo].[DeleteCustomField] 
	@FieldID uniqueidentifier,
	@FieldCat tinyint
AS

DECLARE @WorkflowConditionCount int;
DECLARE @CustEntViewCount int;

BEGIN
	SET @WorkflowConditionCount = (SELECT count(conditionID) FROM workflowConditions WHERE fieldID = @FieldID);
	IF @WorkflowConditionCount > 0
	BEGIN
		RETURN -6; --Assigned to a workflow condition
	END
	
	if @FieldCat <> 3 --If field type is AliasTableField then this check should be ignored 
	BEGIN
		SET @CustEntViewCount = (SELECT count(viewID) FROM custom_entity_view_fields WHERE fieldid = @FieldID);
		IF @CustEntViewCount > 0
		BEGIN
			RETURN -7; --Assigned to a custom entity view
		END
	END		

	DELETE FROM custom_fields WHERE fieldid = @FieldID;
	RETURN 0;
END
