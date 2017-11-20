
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrAssignmentCostings] @list ApiBatchSaveEsrAssignmentCostingsType READONLY
AS
BEGIN
	DECLARE @index BIGINT
	DECLARE @count BIGINT
	DECLARE @ESRCostingAllocationId BIGINT
	DECLARE @ESRPersonId BIGINT
	DECLARE @ESRAssignmentId BIGINT
	DECLARE @EffectiveStartDate DATETIME
	DECLARE @EffectiveEndDate DATETIME
	DECLARE @EntityCode NVARCHAR(3)
	DECLARE @CharitableIndicator NVARCHAR(1)
	DECLARE @CostCentre NVARCHAR(15)
	DECLARE @Subjective NVARCHAR(15)
	DECLARE @Analysis1 NVARCHAR(15)
	DECLARE @Analysis2 NVARCHAR(15)
	DECLARE @ElementNumber INT
	DECLARE @SpareSegment NVARCHAR(60)
	DECLARE @PercentageSplit DECIMAL(5, 2)
	DECLARE @ESRLastUpdate DATETIME
	DECLARE @EsrAssignId INT
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,ESRCostingAllocationId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY ESRCostingAllocationId
			)
		,ESRCostingAllocationId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SELECT TOP 1 @ESRCostingAllocationId = ESRCostingAllocationId
		FROM @tmp
		WHERE tmpID = @index;

		SELECT TOP 1 @ESRPersonId = ESRPersonId
			,@ESRAssignmentId = ESRAssignmentId
			,@EffectiveStartDate = EffectiveStartDate
			,@EffectiveEndDate = EffectiveEndDate
			,@CharitableIndicator = CharitableIndicator
			,@CostCentre = CostCentre
			,@Subjective = Subjective
			,@Analysis1 = Analysis1
			,@Analysis2 = Analysis2
			,@ElementNumber = ElementNumber
			,@SpareSegment = SpareSegment
			,@PercentageSplit = PercentageSplit
			,@ESRLastUpdate = ESRLastUpdate
			,@EsrAssignId = EsrAssignId
		FROM @list
		WHERE ESRCostingAllocationId = @ESRCostingAllocationId

		IF NOT EXISTS (
				SELECT COSTCODEID
				FROM costcodes
				WHERE costcode = @CostCentre
				)
		BEGIN
			DECLARE @costcodeid INT = 0;

			EXEC APIsaveCostcode @costcodeid
				,@CostCentre
				,@CostCentre
				,0
				,NULL
				,NULL
				,NULL
				,NULL;
		END

		IF NOT EXISTS (
				SELECT ESRCostingAllocationId
				FROM ESRAssignmentCostings
				WHERE ESRCostingAllocationId = @ESRCostingAllocationId
				)
		BEGIN
			INSERT INTO [dbo].[ESRAssignmentCostings] (
				[ESRCostingAllocationId]
				,[ESRPersonId]
				,[ESRAssignmentId]
				,[EffectiveStartDate]
				,[EffectiveEndDate]
				,[EntityCode]
				,[CharitableIndicator]
				,[CostCentre]
				,[Subjective]
				,[Analysis1]
				,[Analysis2]
				,[ElementNumber]
				,[SpareSegment]
				,[PercentageSplit]
				,[ESRLastUpdate]
				,[ESRAssignId]
				)
			VALUES (
				@ESRCostingAllocationId
				,@ESRPersonId
				,@ESRAssignmentId
				,@EffectiveStartDate
				,@EffectiveEndDate
				,@EntityCode
				,@CharitableIndicator
				,@CostCentre
				,@Subjective
				,@Analysis1
				,@Analysis2
				,@ElementNumber
				,@SpareSegment
				,@PercentageSplit
				,@ESRLastUpdate
				,@EsrAssignId
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESRAssignmentCostings]
			SET [ESRCostingAllocationId] = @ESRCostingAllocationId
				,[ESRPersonId] = @ESRPersonId
				,[ESRAssignmentId] = @ESRAssignmentId
				,[EffectiveStartDate] = @EffectiveStartDate
				,[EffectiveEndDate] = @EffectiveEndDate
				,[EntityCode] = @EntityCode
				,[CharitableIndicator] = @CharitableIndicator
				,[CostCentre] = @CostCentre
				,[Subjective] = @Subjective
				,[Analysis1] = @Analysis1
				,[Analysis2] = @Analysis2
				,[ElementNumber] = @ElementNumber
				,[SpareSegment] = @SpareSegment
				,[PercentageSplit] = @PercentageSplit
				,[ESRLastUpdate] = @ESRLastUpdate
				,[ESRAssignId] = @EsrAssignId
			WHERE [ESRCostingAllocationId] = @ESRCostingAllocationId
		END

		SET @index = @index + 1
	END

	EXEC APIupdateCostCodes

	RETURN 0;
END