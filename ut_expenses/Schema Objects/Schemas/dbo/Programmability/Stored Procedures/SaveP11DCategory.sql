CREATE PROCEDURE [dbo].[SaveP11DCategory] (
	@p11DCategoryId INT
	,@p11DCategoryName NVARCHAR(50)
	,@userId INT
	,@savedDate DATETIME
	,@CUemployeeID INT
	,@CUdelegateID INT
	)
AS
DECLARE @count INT;

BEGIN
	IF (@p11dcategoryid = 0)
	BEGIN
		SET @count = (
				SELECT COUNT(*)
				FROM pdcats
				WHERE [pdname] = @p11DCategoryName
				);

		IF @count > 0
			RETURN - 1;

		INSERT INTO pdcats (
			pdname
			,CreatedBy
			,CreatedOn
			)
		VALUES (
			@p11DCategoryName
			,@userId
			,@savedDate
			)

		SET @p11DCategoryId = SCOPE_IDENTITY();

		IF @CUemployeeID > 0
		BEGIN
			EXEC addInsertEntryToAuditLog @CUemployeeID
				,@CUdelegateID
				,3
				,@p11DCategoryId
				,@p11DCategoryName
				,NULL;
		END
	END
	ELSE
	BEGIN
		SET @count = (
				SELECT COUNT(*)
				FROM pdcats
				WHERE pdname = @p11DCategoryName
					AND pdcatid <> @p11DCategoryId
				);

		IF @count > 0
			RETURN - 1

		DECLARE @oldP11DCategoryName NVARCHAR(50) = (
				SELECT pdname
				FROM pdcats
				WHERE pdcatid = @p11DCategoryId
				)

		UPDATE pdcats
		SET pdname = @p11DCategoryName
			,ModifiedBy = @userId
			,ModifiedOn = @savedDate
		WHERE pdcatid = @p11DCategoryId

		IF @oldP11DCategoryName <> @p11DCategoryName
			EXEC addUpdateEntryToAuditLog @CUemployeeID
				,@CUdelegateID
				,41
				,@p11DCategoryId
				,'E9B28E60-5AC5-47D0-BCB2-66195B2A5901'
				,@oldP11DCategoryName
				,@p11DCategoryName
				,@p11DCategoryName
				,NULL;
	END

	RETURN @p11DCategoryId;
END