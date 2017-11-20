CREATE PROCEDURE [dbo].[SaveJoinVia]
	@joinViaID INT,
	@joinViaDescription NVARCHAR(MAX),
	@joinViaPathHash CHAR(32),
	@joinViaParts_Int_Unique_TinyInt Int_UniqueIdentifier_TinyInt READONLY
AS
	IF @joinViaID > 0
		BEGIN
			return @joinViaID; -- can't update joinVias
		END
	
	DECLARE @viaID INT;
	DECLARE @viaIDcount INT;
	SET @viaID = 0;
	SET @viaIDcount = 0;

	SELECT @viaID = joinViaID, @viaIDcount = COUNT(joinViaID) FROM joinVia WHERE joinViaPathHash = @joinViaPathHash GROUP BY [joinViaID];
	IF @viaIDcount > 1
		BEGIN
			return -1; -- there shouldn't be more than one record with the PathHash
		END
	ELSE IF @viaIDcount = 1
		BEGIN
			return @viaID; -- should not duplicate paths
		END

	INSERT INTO [joinVia] ([joinViaAS], [joinViaDescription], [joinViaPathHash], [CacheExpiry]) VALUES (NEWID(), @joinViaDescription, @joinViaPathHash, GETUTCDATE());
	SET @viaID = SCOPE_IDENTITY();
	
	INSERT INTO [joinViaParts] ([joinViaID], [relatedID], [relatedType], [order]) SELECT @viaID, c2, c3, c1 FROM @joinViaParts_Int_Unique_TinyInt ORDER BY c1;

RETURN @viaID;
