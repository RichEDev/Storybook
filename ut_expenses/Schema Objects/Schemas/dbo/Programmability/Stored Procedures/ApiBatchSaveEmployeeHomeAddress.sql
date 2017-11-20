
CREATE PROCEDURE [dbo].[ApiBatchSaveEmployeeHomeAddress] @list ApiBatchSaveEmployeeHomeAddressType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @EmployeeHomeAddressId INT
	DECLARE @EmployeeId INT
	DECLARE @AddressId INT
	DECLARE @StartDate DATETIME
	DECLARE @EndDate DATETIME
	DECLARE @CreatedOn DATETIME
	DECLARE @CreatedBy INT
	DECLARE @ModifiedOn DATETIME
	DECLARE @ModifiedBy INT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,EmployeeId BIGINT
		,AddressId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY EmployeeId
				,AddressId
			)
		,EmployeeId
		,AddressId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SELECT TOP 1 @EmployeeId = EmployeeId
			,@AddressId = AddressId
		FROM @tmp
		WHERE tmpID = @index;

		SELECT TOP 1 @EmployeeHomeAddressId = EmployeeHomeAddressId
			,@StartDate = StartDate
			,@EndDate = EndDate
			,@CreatedOn = CreatedOn
			,@CreatedBy = CreatedBy
			,@ModifiedOn = ModifiedOn
			,@ModifiedBy = ModifiedBy
		FROM @list
		WHERE EmployeeId = @EmployeeId
			AND AddressId = @AddressId

		IF @EmployeeHomeAddressId = 0
		BEGIN
			DECLARE @currentId INT = NULL;

			SELECT @currentId = EmployeeHomeAddressId
			FROM [dbo].[employeeHomeAddresses]
			WHERE AddressId = @AddressId
				AND EmployeeId = @EmployeeId;

			IF @currentId IS NULL
			BEGIN
				INSERT INTO [dbo].[employeeHomeAddresses] (
					[EmployeeId]
					,[AddressId]
					,[StartDate]
					,[EndDate]
					,[CreatedOn]
					,[CreatedBy]
					)
				VALUES (
					@EmployeeId
					,@AddressId
					,@StartDate
					,@EndDate
					,@CreatedOn
					,@CreatedBy
					);

				SET @EmployeeHomeAddressId = SCOPE_IDENTITY();
			END
			ELSE
			BEGIN
				SET @EmployeeHomeAddressId = @currentId;

				UPDATE [dbo].[employeeHomeAddresses]
				SET [StartDate] = @StartDate
					,[EndDate] = @EndDate
					,[ModifiedOn] = @ModifiedOn
					,[ModifiedBy] = @ModifiedBy
				WHERE EmployeeHomeAddressId = @EmployeeHomeAddressId;
			END
		END
		ELSE
		BEGIN
			UPDATE [dbo].[employeeHomeAddresses]
			SET [EmployeeId] = @EmployeeId
				,[AddressId] = @AddressId
				,[StartDate] = @StartDate
				,[EndDate] = @EndDate
				,[ModifiedOn] = @ModifiedOn
				,[ModifiedBy] = @ModifiedBy
			WHERE EmployeeHomeAddressId = @EmployeeHomeAddressId;
		END

		SET @index = @index + 1
	END

	RETURN 0;
END