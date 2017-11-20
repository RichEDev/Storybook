CREATE PROC GetChildFilterFieldValues
@parentControlId NVARCHAR(50),
@ChildControlId NVARCHAR(50)
AS
BEGIN
IF EXISTS(SELECT * FROM fieldFilters WHERE  isParentFilter=1 AND attributeid=@ChildControlId AND [value]=@parentControlId)
BEGIN
SELECT relationshipDisplayField FROM customEntityAttributes WHERE attributeid =@parentControlId
END
END