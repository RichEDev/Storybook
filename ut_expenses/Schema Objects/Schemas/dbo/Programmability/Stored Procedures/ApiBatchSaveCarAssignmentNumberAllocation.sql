
CREATE PROCEDURE [dbo].[ApiBatchSaveCarAssignmentNumberAllocation] @list ApiBatchSaveCarAssignmentNumberAllocationType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @ESRVehicleAllocationId BIGINT
	DECLARE @ESRAssignId INT
	DECLARE @CarId INT
	DECLARE @Archived BIT
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

		SELECT TOP 1 @ESRAssignId = ESRAssignId
			,@CarId = CarId
			,@Archived = Archived
		FROM @list
		WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId

		IF NOT EXISTS (
				SELECT EsrVehicleAllocationId
				FROM CarAssignmentNumberAllocations
				WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId
				)
		BEGIN
			INSERT INTO CarAssignmentNumberAllocations (
				ESRVehicleAllocationId
				,ESRAssignId
				,CarId
				,Archived
				)
			VALUES (
				@ESRVehicleAllocationId
				,@ESRAssignId
				,@CarId
				,@Archived
				)
		END
		ELSE
		BEGIN
			UPDATE CarAssignmentNumberAllocations
			SET ESRVehicleAllocationId = @ESRVehicleAllocationId
				,ESRAssignId = @ESRAssignId
				,CarId = @CarId
				,Archived = @Archived
			WHERE ESRVehicleAllocationId = @ESRVehicleAllocationId
		END

		SET @index = @index + 1
	END

	RETURN 0;
END