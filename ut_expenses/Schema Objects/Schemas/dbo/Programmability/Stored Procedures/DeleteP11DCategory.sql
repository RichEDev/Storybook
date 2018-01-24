CREATE PROCEDURE [dbo].[DeleteP11DCategory] (
	@p11DCategoryId INT
	,@CUemployeeID INT
	,@CUdelegateID INT
	)
AS
BEGIN
	DECLARE @tableId UNIQUEIDENTIFIER = (
			SELECT tableid
			FROM tables
			WHERE tablename = 'pdcats'
			)
	DECLARE @returnVal INT

	EXEC @returnVal = checkReferencedBy @tableId
		,@p11DCategoryId

	IF (@returnVal = - 10)
		RETURN - 10;

	UPDATE subcats
	SET pdcatid = NULL
	WHERE pdcatid = @p11DCategoryId

	DECLARE @p11DCategoryName NVARCHAR(50) = (
			SELECT pdname
			FROM pdcats
			WHERE pdcatid = @p11DCategoryId
			)

	DELETE
	FROM pdcats
	WHERE pdcatid = @p11DCategoryId

	EXEC addDeleteEntryToAuditLog @CUemployeeID
		,@CUdelegateID
		,41
		,@p11DCategoryId
		,@p11DCategoryName
		,NULL;

	RETURN 0;
END