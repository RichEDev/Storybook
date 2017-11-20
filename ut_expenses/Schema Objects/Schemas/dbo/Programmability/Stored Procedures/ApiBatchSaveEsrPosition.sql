
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrPosition] @list ApiBatchSaveEsrPositionType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @ESRPositionId BIGINT
	DECLARE @EffectiveFromDate DATETIME
	DECLARE @EffectiveToDate DATETIME
	DECLARE @PositionNumber BIGINT
	DECLARE @PositionName NVARCHAR(240)
	DECLARE @BudgetedFTE DECIMAL(13, 5)
	DECLARE @SubjectiveCode NVARCHAR(15)
	DECLARE @JobStaffGroup NVARCHAR(40)
	DECLARE @JobRole NVARCHAR(60)
	DECLARE @OccupationCode NVARCHAR(5)
	DECLARE @Payscale NVARCHAR(10)
	DECLARE @GradeStep NVARCHAR(30)
	DECLARE @ISARegulatedPost NVARCHAR(15)
	DECLARE @ESROrganisationId BIGINT
	DECLARE @HiringStatus NVARCHAR(80)
	DECLARE @PositionType NVARCHAR(80)
	DECLARE @OHProcessingEligible NVARCHAR(30)
	DECLARE @EPPFlag NVARCHAR(30)
	DECLARE @DeaneryPostNumber NVARCHAR(30)
	DECLARE @ManagingDeaneryBody NVARCHAR(10)
	DECLARE @WorkplaceOrgCode NVARCHAR(10)
	DECLARE @ESRLastUpdateDate DATETIME
	DECLARE @SubjectiveCodeDescription NVARCHAR(240)
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,EsrPositionId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY EsrPositionId
			)
		,EsrPositionId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @EsrPositionId = (
				SELECT TOP 1 EsrPositionId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @EffectiveFromDate = EffectiveFromDate
			,@EffectiveToDate = EffectiveToDate
			,@PositionNumber = PositionNumber
			,@PositionName = PositionName
			,@BudgetedFTE = BudgetedFTE
			,@SubjectiveCode = SubjectiveCode
			,@JobStaffGroup = JobStaffGroup
			,@JobRole = JobRole
			,@OccupationCode = OccupationCode
			,@Payscale = Payscale
			,@GradeStep = GradeStep
			,@ISARegulatedPost = ISARegulatedPost
			,@ESROrganisationId = ESROrganisationId
			,@HiringStatus = HiringStatus
			,@PositionType = PositionType
			,@OHProcessingEligible = OHProcessingEligible
			,@EPPFlag = EPPFlag
			,@DeaneryPostNumber = DeaneryPostNumber
			,@ManagingDeaneryBody = ManagingDeaneryBody
			,@WorkplaceOrgCode = WorkplaceOrgCode
			,@ESRLastUpdateDate = ESRLastUpdateDate
			,@SubjectiveCodeDescription = SubjectiveCodeDescription
		FROM @list
		WHERE EsrPositionId = @EsrPositionId

		IF NOT EXISTS (
				SELECT ESRPositionId
				FROM ESRPositions
				WHERE ESRPositionId = @ESRPositionId
				)
		BEGIN
			INSERT INTO [dbo].[ESRPositions] (
				[ESRPositionId]
				,[EffectiveFromDate]
				,[EffectiveToDate]
				,[PositionNumber]
				,[PositionName]
				,[BudgetedFTE]
				,[SubjectiveCode]
				,[JobStaffGroup]
				,[JobRole]
				,[OccupationCode]
				,[Payscale]
				,[GradeStep]
				,[ISARegulatedPost]
				,[ESROrganisationId]
				,[HiringStatus]
				,[PositionType]
				,[OHProcessingEligible]
				,[EPPFlag]
				,[DeaneryPostNumber]
				,[ManagingDeaneryBody]
				,[WorkplaceOrgCode]
				,[ESRLastUpdateDate]
				,[SubjectiveCodeDescription]
				)
			VALUES (
				@ESRPositionId
				,@EffectiveFromDate
				,@EffectiveToDate
				,@PositionNumber
				,@PositionName
				,@BudgetedFTE
				,@SubjectiveCode
				,@JobStaffGroup
				,@JobRole
				,@OccupationCode
				,@Payscale
				,@GradeStep
				,@ISARegulatedPost
				,@ESROrganisationId
				,@HiringStatus
				,@PositionType
				,@OHProcessingEligible
				,@EPPFlag
				,@DeaneryPostNumber
				,@ManagingDeaneryBody
				,@WorkplaceOrgCode
				,@ESRLastUpdateDate
				,@SubjectiveCodeDescription
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESRPositions]
			SET [ESRPositionId] = @ESRPositionId
				,[EffectiveFromDate] = @EffectiveFromDate
				,[EffectiveToDate] = @EffectiveToDate
				,[PositionNumber] = @PositionNumber
				,[PositionName] = @PositionName
				,[BudgetedFTE] = @BudgetedFTE
				,[SubjectiveCode] = @SubjectiveCode
				,[JobStaffGroup] = @JobStaffGroup
				,[JobRole] = @JobRole
				,[OccupationCode] = @OccupationCode
				,[Payscale] = @Payscale
				,[GradeStep] = @GradeStep
				,[ISARegulatedPost] = @ISARegulatedPost
				,[ESROrganisationId] = @ESROrganisationId
				,[HiringStatus] = @HiringStatus
				,[PositionType] = @PositionType
				,[OHProcessingEligible] = @OHProcessingEligible
				,[EPPFlag] = @EPPFlag
				,[DeaneryPostNumber] = @DeaneryPostNumber
				,[ManagingDeaneryBody] = @ManagingDeaneryBody
				,[WorkplaceOrgCode] = @WorkplaceOrgCode
				,[ESRLastUpdateDate] = @ESRLastUpdateDate
				,[SubjectiveCodeDescription] = @SubjectiveCodeDescription
			WHERE ESRPositionId = @ESRPositionId
		END

		SET @index = @index + 1
	END

	RETURN 0;
END