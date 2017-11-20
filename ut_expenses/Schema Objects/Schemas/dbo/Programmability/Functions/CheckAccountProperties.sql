CREATE FUNCTION CheckAccountProperties (
	@StringKeyToFind NVARCHAR(max)
	,@ValueToFind NVARCHAR(max)
	)
RETURNS BIT
AS
BEGIN
	DECLARE @RecordCount INT

	SELECT @RecordCount = count(1)
	FROM accountProperties
	WHERE stringkey = @StringKeyToFind
		AND stringValue = @ValueToFind

	IF (@RecordCount <> 0)
		RETURN 1
	
	RETURN 0
END
GO