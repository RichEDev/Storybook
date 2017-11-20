CREATE PROCEDURE DeleteJoinVias
	@joinViaIDs IntPK READONLY
AS
BEGIN
	DECLARE @affectedRows INT = 0;
	DECLARE @jvCount INT = 0;
	
	SELECT @jvCount = COUNT(c1) FROM @joinViaIDs;
	
	-- delete entries or rows on each table that reference the joinvia
	DELETE FROM employeeGridSortOrders WHERE sortJoinViaID = (SELECT c1 FROM @joinViaIDs);
	SET @affectedRows = @@ROWCOUNT;
	
	UPDATE customEntityViews SET SortColumn = CAST('00000000-0000-0000-0000-000000000000' AS UNIQUEIDENTIFIER), SortOrder = 0, SortColumnJoinViaID = NULL, CacheExpiry = GETUTCDATE() WHERE SortColumnJoinViaID = (SELECT c1 FROM @joinViaIDs);
	SET @affectedRows = @affectedRows + @@ROWCOUNT;
	
	DELETE FROM customEntityViewFields WHERE joinViaID = (SELECT c1 FROM @joinViaIDs);
	SET @affectedRows = @affectedRows + @@ROWCOUNT;
	
	DELETE FROM [fieldFilters] WHERE joinViaID = (SELECT c1 FROM @joinViaIDs);
	SET @affectedRows = @affectedRows + @@ROWCOUNT;
	
	-- Finally clear up the joinvias
	DELETE FROM joinVia WHERE joinViaID = (SELECT c1 FROM @joinViaIDs);
	
	IF @jvCount = @@ROWCOUNT
	BEGIN
		IF @affectedRows = 0
		BEGIN
			RETURN 0; -- deleted all JoinVias requested and nothing else was affected
		END
		ELSE
		BEGIN
			RETURN -1; -- deleted all JoinVias requested and other areas were affected
		END
	END
	ELSE
	BEGIN
		IF @affectedRows = 0
		BEGIN
			RETURN -2; -- didn't delete all JoinVias requested and nothing else was affected
		END
		ELSE
		BEGIN
			RETURN -3; -- didn't delete all JoinVias requested and other areas were affected
		END
	END
END
