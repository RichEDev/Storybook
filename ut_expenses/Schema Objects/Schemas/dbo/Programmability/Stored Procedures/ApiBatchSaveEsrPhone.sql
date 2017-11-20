
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrPhone] @list ApiBatchSaveEsrPhoneType READONLY
AS
BEGIN
	DECLARE @ESRPhoneId BIGINT
		,@ESRPersonId BIGINT
		,@PhoneType NVARCHAR(500)
		,@PhoneNumber NVARCHAR(500)
		,@EffectiveStartDate DATETIME
		,@EffectiveEndDate DATETIME
		,@ESRLastUpdate DATETIME
		,@index BIGINT
		,@count BIGINT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,ESRPhoneId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY ESRPhoneId
			)
		,ESRPhoneId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESRPhoneId = (
				SELECT TOP 1 ESRPhoneId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @ESRPersonId = ESRPersonId
			,@PhoneType = PhoneType
			,@PhoneNumber = PhoneNumber
			,@EffectiveStartDate = EffectiveStartDate
			,@EffectiveEndDate = EffectiveEndDate
			,@ESRLastUpdate = ESRLastUpdate
		FROM @list
		WHERE ESRPhoneId = @ESRPhoneId

		IF NOT EXISTS (
				SELECT ESRPhoneId
				FROM [dbo].[ESRPhones]
				WHERE ESRPhoneId = @ESRPhoneId
				)
		BEGIN
			INSERT INTO [dbo].[ESRPhones] (
				[ESRPhoneId]
				,[ESRPersonId]
				,[PhoneType]
				,[PhoneNumber]
				,[EffectiveStartDate]
				,[EffectiveEndDate]
				,[ESRLastUpdate]
				)
			VALUES (
				@ESRPhoneId
				,@ESRPersonId
				,@PhoneType
				,@PhoneNumber
				,@EffectiveStartDate
				,@EffectiveEndDate
				,@ESRLastUpdate
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESRPhones]
			SET [ESRPhoneId] = @ESRPhoneId
				,[ESRPersonId] = @ESRPersonId
				,[PhoneType] = @PhoneType
				,[PhoneNumber] = @PhoneNumber
				,[EffectiveStartDate] = @EffectiveStartDate
				,[EffectiveEndDate] = @EffectiveEndDate
				,[ESRLastUpdate] = @ESRLastUpdate
			WHERE [ESRPhoneId] = @ESRPhoneId
		END

		SET @index = @index + 1
	END

	RETURN 0;
END