
CREATE PROCEDURE [dbo].[ApiBatchSaveCostcode] @list ApiBatchSaveCostcodeType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @costcodeid INT
	DECLARE @costcode NVARCHAR(50)
	DECLARE @description NVARCHAR(4000)
	DECLARE @archived BIT
	DECLARE @CreatedOn DATETIME
	DECLARE @CreatedBy DATETIME
	DECLARE @ModifiedOn DATETIME
	DECLARE @ModifiedBy DATETIME
	DECLARE @counter INT;
	DECLARE @usingdescription NVARCHAR(10)

	SELECT @usingdescription = stringValue
	FROM accountProperties
	WHERE stringKey = 'useCostCodeDescription'
		AND subAccountID IN (
			SELECT TOP 1 subAccountID
			FROM accountsSubAccounts
			)

	DECLARE @tmp TABLE (
		tmpID BIGINT
		,costcode NVARCHAR(50)
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY costcode
			)
		,costcode
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @costcode = (
				SELECT TOP 1 costcode
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @costcodeid = costcode
			,@description = [description]
			,@archived = archived
			,@CreatedOn = CreatedOn
			,@CreatedBy = CreatedBy
			,@ModifiedOn = ModifiedOn
			,@ModifiedBy = ModifiedBy
		FROM @list
		WHERE costcode = @costcode

		IF @costcodeid = 0
		BEGIN
			SELECT @counter = costcodeid
			FROM costcodes
			WHERE costcode = @costcode;

			IF @counter IS NOT NULL
			BEGIN
				SET @costcodeid = @counter;
			END

			IF @usingdescription = '1'
			BEGIN
				SELECT @counter = costcodeid
				FROM costcodes
				WHERE description = @description

				IF @counter IS NOT NULL
				BEGIN
					SET @costcodeid = @counter
				END
			END
		END

		IF @costcodeid = 0
		BEGIN
			INSERT INTO costcodes (
				costcode
				,[description]
				,createdon
				)
			VALUES (
				@costcode
				,@description
				,GETUTCDATE()
				);

			SET @costcodeid = scope_identity()
		END
		ELSE
		BEGIN
			SET @counter = (
					SELECT COUNT(*)
					FROM costcodes
					WHERE costcode = @costcode
						AND costcodeid <> @costcodeid
					);

			IF @counter > 0
				RETURN - 1;

			IF @usingdescription = '1'
			BEGIN
				SET @counter = (
						SELECT count(*)
						FROM costcodes
						WHERE description = @description
							AND costcodeid <> @costcodeid
						)

				IF @counter > 0
					RETURN - 2
			END

			UPDATE costcodes
			SET costcode = @costcode
				,[description] = @description
			WHERE [costcodeid] = @costcodeid
		END

		SET @index = @index + 1
	END

	RETURN 0;
END