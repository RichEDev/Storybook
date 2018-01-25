CREATE PROCEDURE [dbo].[GetChildFilterFieldValues] 
@parentControlId NVARCHAR(50)
,@ChildControlId NVARCHAR(50)
AS
BEGIN
	IF EXISTS (
			SELECT *
			FROM fieldFilters
			WHERE isParentFilter = 1
				AND attributeid = @ChildControlId
				AND [value] = @parentControlId
			)
	BEGIN
		SELECT primarykey
		FROM [tables]
		INNER JOIN customEntityAttributes ON [tables].tableid = customEntityAttributes.relatedtable
		WHERE customEntityAttributes.attributeid = @parentControlId
	END
END