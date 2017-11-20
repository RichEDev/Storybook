
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrVehicle] @list ApiBatchSaveEsrVehicleType READONLY
AS
BEGIN
	DECLARE @ESRVehicleAllocationId BIGINT
		,@ESRPersonId BIGINT
		,@ESRAssignmentId BIGINT
		,@EffectiveStartDate DATETIME
		,@EffectiveEndDate DATETIME
		,@RegistrationNumber NVARCHAR(500)
		,@Make NVARCHAR(500)
		,@Model NVARCHAR(500)
		,@Ownership NVARCHAR(500)
		,@InitialRegistrationDate DATETIME
		,@EngineCC DECIMAL(12, 3)
		,@ESRLastUpdate DATETIME
		,@UserRatesTable NVARCHAR(500)
		,@FuelType NVARCHAR(500)
		,@ESRAssignId INT
		,@index BIGINT
		,@count BIGINT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,ESRVehicleAllocationId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY ESRVehicleAllocationId
			)
		,ESRVehicleAllocationId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESRVehicleAllocationId = (
				SELECT TOP 1 ESRVehicleAllocationId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @ESRPersonId = ESRPersonId
			,@ESRPersonId = ESRPersonId
			,@ESRAssignmentId = ESRAssignmentId
			,@EffectiveStartDate = EffectiveStartDate
			,@EffectiveEndDate = EffectiveEndDate
			,@RegistrationNumber = RegistrationNumber
			,@Make = Make
			,@Model = Model
			,@Ownership = [Ownership]
			,@InitialRegistrationDate = InitialRegistrationDate
			,@EngineCC = EngineCC
			,@ESRLastUpdate = ESRLastUpdate
			,@UserRatesTable = UserRatesTable
			,@FuelType = FuelType
			,@ESRAssignId = ESRAssignId
		FROM @list
		WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId

		IF NOT EXISTS (
				SELECT ESRVehicleAllocationId
				FROM ESRVehicles
				WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId
				)
		BEGIN
			INSERT INTO [dbo].[ESRVehicles] (
				[ESRVehicleAllocationId]
				,[ESRPersonId]
				,[ESRAssignmentId]
				,[EffectiveStartDate]
				,[EffectiveEndDate]
				,[RegistrationNumber]
				,[Make]
				,[Model]
				,[Ownership]
				,[InitialRegistrationDate]
				,[EngineCC]
				,[ESRLastUpdate]
				,[UserRatesTable]
				,[FuelType]
				,[ESRAssignId]
				)
			VALUES (
				@ESRVehicleAllocationId
				,@ESRPersonId
				,@ESRAssignmentId
				,@EffectiveStartDate
				,@EffectiveEndDate
				,@RegistrationNumber
				,@Make
				,@Model
				,@Ownership
				,@InitialRegistrationDate
				,@EngineCC
				,@ESRLastUpdate
				,@UserRatesTable
				,@FuelType
				,@ESRAssignId
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESRVehicles]
			SET [ESRVehicleAllocationId] = @ESRVehicleAllocationId
				,[ESRPersonId] = @ESRPersonId
				,[ESRAssignmentId] = @ESRAssignmentId
				,[EffectiveStartDate] = @EffectiveStartDate
				,[EffectiveEndDate] = @EffectiveEndDate
				,[RegistrationNumber] = @RegistrationNumber
				,[Make] = @Make
				,[Model] = @Model
				,[Ownership] = @Ownership
				,[InitialRegistrationDate] = @InitialRegistrationDate
				,[EngineCC] = @EngineCC
				,[ESRLastUpdate] = @ESRLastUpdate
				,[UserRatesTable] = @UserRatesTable
				,[FuelType] = @FuelType
				,[ESRAssignId] = @ESRAssignId
			WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId
		END

		SET @index = @index + 1
	END

	RETURN 0;
END