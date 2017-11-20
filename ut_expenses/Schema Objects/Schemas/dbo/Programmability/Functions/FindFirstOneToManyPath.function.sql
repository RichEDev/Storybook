CREATE FUNCTION [dbo].[FindFirstOneToManyPath] 
(
	-- Add the parameters for the function here
	@baseTableID UNIQUEIDENTIFIER, 
	@targetTableID UNIQUEIDENTIFIER
)
RETURNS 
@oneToManyPath TABLE 
(
	-- Add the column definitions for the TABLE variable here
	[sourceTableID] UNIQUEIDENTIFIER, 
	[sourceFieldID] UNIQUEIDENTIFIER,
	[destinationTableID] UNIQUEIDENTIFIER, 
	[destinationFieldID] UNIQUEIDENTIFIER,
	[stepSequence] TINYINT
)
AS
BEGIN
	-- validate inputs
	IF NOT EXISTS (SELECT [tableid] FROM [customEntities] WHERE [tableId] = @baseTableID)
		OR NOT EXISTS (SELECT [tableid] FROM [customEntities] WHERE [tableId] = @targetTableID)
	BEGIN
		RETURN
	END

	-- find any relationships that point to the target entity,
	-- see if their ce's tableid matches the basetable,
	-- if it does we're done, add it to the top of the steps and return
	-- if not see if the next step up does
	
	DECLARE @count int = 0;
	DECLARE @isSystemView BIT = 0;
	DECLARE @sourceTableID UNIQUEIDENTIFIER = NULL;
	DECLARE @sourceFieldID UNIQUEIDENTIFIER = NULL;
	DECLARE @sourceEntityID INT = 0;
	DECLARE @destinationTableID UNIQUEIDENTIFIER = @targetTableID;
	DECLARE @destinationFieldID UNIQUEIDENTIFIER = NULL;
	DECLARE @destinationEntityID INT = 0;

	SELECT	@isSystemView = [systemview], 
			@destinationEntityID = [entityId] 
		FROM [customEntities]
		WHERE [tableId] = @destinationTableID;

	IF @isSystemView = 1
	BEGIN
		SET @destinationEntityID = (SELECT [systemview_entityid] FROM [customEntities] WHERE [systemview] = 1 AND [tableid] = @destinationTableID);
	END
	
	DECLARE otms_cursor CURSOR LOCAL FAST_FORWARD
	FOR
		SELECT	[customEntities].[tableid],
				[customEntities].[entityId],
				[customEntityAttributes].[fieldId]
			FROM [customEntityAttributes] 
			INNER JOIN [customEntities] ON [customEntityAttributes].[entityid] = [customEntities].[entityid]
			WHERE [customEntityAttributes].[fieldtype] = 9
				AND [customEntityAttributes].[relationshipType] = 2
				AND (
					(@isSystemView = 0 AND [customEntityAttributes].[related_entity] = @destinationEntityID)
					OR
					(@isSystemView = 1 AND [customEntityAttributes].[entityid] = @destinationEntityID)
				);
				
	OPEN otms_cursor;
	FETCH NEXT FROM otms_cursor INTO @sourceTableID, @sourceEntityID, @destinationFieldID;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- get the primary key field for the source table
		SET @sourceFieldID = (SELECT fieldId FROM customEntityAttributes WHERE is_key_field = 1 AND entityid = @sourceEntityID);
		
		-- validate ids
		IF @sourceFieldID IS NULL OR @sourceTableID IS NULL OR @destinationFieldID IS NULL OR @destinationTableID IS NULL
		BEGIN
			FETCH NEXT FROM otms_cursor INTO @sourceTableID, @sourceEntityID, @destinationFieldID;
			CONTINUE;
		END
		
		
		IF @sourceTableID = @baseTableID
		BEGIN
			-- if we've found the base table, return it
			INSERT INTO @oneToManyPath VALUES (@sourceTableID, @sourceFieldID, @destinationTableID, @destinationFieldID, 1);
			BREAK;
		END
		ELSE
		BEGIN
			-- if we've reached 32 levels of recursion then we can go no further, unwind before we error
			-- unfortunately this means an empty set will be returned, but should also stop loops
			IF @@NESTLEVEL = 32
			BEGIN
				INSERT INTO @oneToManyPath VALUES ('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', 0);
				FETCH NEXT FROM otms_cursor INTO @sourceTableID, @sourceEntityID, @destinationFieldID;
				CONTINUE;
			END
			
			-- otherwise search further up the tree
			-- this should only return a value if it's the base table or on the path to it
			INSERT INTO @oneToManyPath SELECT * FROM dbo.FindFirstOneToManyPath(@baseTableID, @sourceTableID);
			
			-- see if anything was found
			SET @count = (SELECT COUNT(*) FROM @oneToManyPath WHERE stepSequence > 0);
			
			IF @count > 0
			BEGIN
				-- only add on this step if the base table was found above and then return
				INSERT INTO @oneToManyPath VALUES (@sourceTableID, @sourceFieldID, @destinationTableID, @destinationFieldID, @count + 1);
				BREAK;
			END
		END
		
		-- otherwise explore the next candidate - @oneToManyPath should still be empty
		FETCH NEXT FROM otms_cursor INTO @sourceTableID, @sourceEntityID, @destinationFieldID;
	END
	
	CLOSE otms_cursor;
	DEALLOCATE otms_cursor;

	RETURN 
END