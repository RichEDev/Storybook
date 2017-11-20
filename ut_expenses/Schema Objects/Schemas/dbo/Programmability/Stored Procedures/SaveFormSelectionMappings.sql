CREATE PROCEDURE [dbo].[SaveFormSelectionMappings]
	@viewId INT,
	@formSelectionMappings dbo.FormSelectionMapping READONLY, 
	@employeeId int,
	@delegateId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT * FROM @formSelectionMappings)
	BEGIN
		DELETE [customEntityViewFormSelectionMappings] WHERE [viewId] = @viewId;
	END

	-- delete items that are no longer there
	DELETE m
	FROM [customEntityViewFormSelectionMappings] m
	WHERE m.[viewId] = @viewId 
		AND [viewFormSelectionMappingId] NOT IN (SELECT [formSelectionMappingId] FROM @formSelectionMappings);
	
	INSERT INTO [customEntityViewFormSelectionMappings]  ([viewId], [isAdd], [formId], [textValue], [listValue])
		SELECT [viewId], [isAdd], [formId], [textValue], [listValue] FROM @formSelectionMappings WHERE [formSelectionMappingId] = 0;
		
	RETURN 0;
END