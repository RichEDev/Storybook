CREATE PROCEDURE DeleteJoinViasUsingRelatedID
	@relatedID uniqueidentifier
AS
BEGIN
	DECLARE @deleteJoinViaIDs AS IntPK;
	DECLARE @count INT = 0;
	DECLARE @retVal INT = 0;
	DECLARE @returnCode INT = 0;
	
	-- find the list of JoinVias that used this attribute/field/table as part of its join
	INSERT INTO @deleteJoinViaIDs SELECT DISTINCT joinViaID FROM joinViaParts WHERE relatedID = @relatedID;
	SET @count = @@ROWCOUNT;
	
	IF @count = 0
		RETURN -11; -- none found
	
	--  Use the list of joinVias found to delete them
	EXEC @retVal = DeleteJoinVias @deleteJoinViaIDs;
	
	RETURN @retVal;
END
