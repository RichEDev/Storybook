
CREATE PROCEDURE [dbo].[ApiBatchSaveEsrPerson] @list ApiBatchSaveEsrPersonType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @ESRPersonId BIGINT
	DECLARE @EffectiveStartDate DATETIME
	DECLARE @EffectiveEndDate DATETIME
	DECLARE @EmployeeNumber NVARCHAR(30)
	DECLARE @Title NVARCHAR(30)
	DECLARE @LastName NVARCHAR(150)
	DECLARE @FirstName NVARCHAR(150)
	DECLARE @MiddleNames NVARCHAR(60)
	DECLARE @MaidenName NVARCHAR(150)
	DECLARE @PreferredName NVARCHAR(80)
	DECLARE @PreviousLastName NVARCHAR(150)
	DECLARE @Gender NVARCHAR(30)
	DECLARE @DateOfBirth DATETIME
	DECLARE @NINumber NVARCHAR(30)
	DECLARE @NHSUniqueId NVARCHAR(15)
	DECLARE @HireDate DATETIME
	DECLARE @ActualTerminationDate DATETIME
	DECLARE @TerminationReason NVARCHAR(30)
	DECLARE @EmployeeStatusFlag NVARCHAR(3)
	DECLARE @WTROptOut NVARCHAR(3)
	DECLARE @WTROptOutDate DATETIME
	DECLARE @EthnicOrigin NVARCHAR(30)
	DECLARE @MaritalStatus NVARCHAR(30)
	DECLARE @CountryOfBirth NVARCHAR(30)
	DECLARE @PreviousEmployer NVARCHAR(240)
	DECLARE @PreviousEmployerType NVARCHAR(30)
	DECLARE @CSD3Months DATETIME
	DECLARE @CSD12Months DATETIME
	DECLARE @NHSCRSUUID NVARCHAR(12)
	DECLARE @SystemPersonType NVARCHAR(30)
	DECLARE @UserPersonType NVARCHAR(80)
	DECLARE @OfficeEmailAddress NVARCHAR(240)
	DECLARE @NHSStartDate DATETIME
	DECLARE @ESRLastUpdateDate DATETIME
	DECLARE @DisabilityFlag NVARCHAR(1)
	DECLARE @LegacyPayrollNumber NVARCHAR(150)
	DECLARE @Nationality NVARCHAR(30)
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,EsrPersonId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY EsrPersonId
			)
		,EsrPersonId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESRPersonId = (
				SELECT TOP 1 ESRPersonId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @EffectiveStartDate = EffectiveStartDate
			,@EffectiveEndDate = EffectiveEndDate
			,@EmployeeNumber = EmployeeNumber
			,@Title = Title
			,@LastName = LastName
			,@FirstName = FirstName
			,@MiddleNames = MiddleNames
			,@MaidenName = MaidenName
			,@PreferredName = PreferredName
			,@PreviousLastName = PreviousLastName
			,@Gender = Gender
			,@DateOfBirth = DateOfBirth
			,@NINumber = NINumber
			,@NHSUniqueId = NHSUniqueId
			,@HireDate = HireDate
			,@ActualTerminationDate = ActualTerminationDate
			,@TerminationReason = TerminationReason
			,@EmployeeStatusFlag = EmployeeStatusFlag
			,@WTROptOut = WTROptOut
			,@WTROptOutDate = WTROptOutDate
			,@EthnicOrigin = EthnicOrigin
			,@MaritalStatus = MaritalStatus
			,@CountryOfBirth = CountryOfBirth
			,@PreviousEmployer = PreviousEmployer
			,@PreviousEmployerType = PreviousEmployerType
			,@CSD3Months = CSD3Months
			,@CSD12Months = CSD12Months
			,@NHSCRSUUID = NHSCRSUUID
			,@SystemPersonType = SystemPersonType
			,@UserPersonType = UserPersonType
			,@OfficeEmailAddress = OfficeEmailAddress
			,@NHSStartDate = NHSStartDate
			,@ESRLastUpdateDate = ESRLastUpdateDate
			,@DisabilityFlag = DisabilityFlag
			,@LegacyPayrollNumber = LegacyPayrollNumber
			,@Nationality = Nationality
		FROM @list
		WHERE ESRPersonId = @ESRPersonId

		IF NOT EXISTS (
				SELECT ESRPersonId
				FROM [dbo].[ESRPersons]
				WHERE ESRPersonId = @ESRPersonId
				)
		BEGIN
			INSERT INTO [dbo].[ESRPersons] (
				[ESRPersonId]
				,[EffectiveStartDate]
				,[EffectiveEndDate]
				,[EmployeeNumber]
				,[Title]
				,[LastName]
				,[FirstName]
				,[MiddleNames]
				,[MaidenName]
				,[PreferredName]
				,[PreviousLastName]
				,[Gender]
				,[DateOfBirth]
				,[NINumber]
				,[NHSUniqueId]
				,[HireDate]
				,[ActualTerminationDate]
				,[TerminationReason]
				,[EmployeeStatusFlag]
				,[WTROptOut]
				,[WTROptOutDate]
				,[EthnicOrigin]
				,[MaritalStatus]
				,[CountryOfBirth]
				,[PreviousEmployer]
				,[PreviousEmployerType]
				,[CSD3Months]
				,[CSD12Months]
				,[NHSCRSUUID]
				,[SystemPersonType]
				,[UserPersonType]
				,[OfficeEmailAddress]
				,[NHSStartDate]
				,[ESRLastUpdateDate]
				,[DisabilityFlag]
				,[LegacyPayrollNumber]
				,[Nationality]
				)
			VALUES (
				@ESRPersonId
				,@EffectiveStartDate
				,@EffectiveEndDate
				,@EmployeeNumber
				,@Title
				,@LastName
				,@FirstName
				,@MiddleNames
				,@MaidenName
				,@PreferredName
				,@PreviousLastName
				,@Gender
				,@DateOfBirth
				,@NINumber
				,@NHSUniqueId
				,@HireDate
				,@ActualTerminationDate
				,@TerminationReason
				,@EmployeeStatusFlag
				,@WTROptOut
				,@WTROptOutDate
				,@EthnicOrigin
				,@MaritalStatus
				,@CountryOfBirth
				,@PreviousEmployer
				,@PreviousEmployerType
				,@CSD3Months
				,@CSD12Months
				,@NHSCRSUUID
				,@SystemPersonType
				,@UserPersonType
				,@OfficeEmailAddress
				,@NHSStartDate
				,@ESRLastUpdateDate
				,@DisabilityFlag
				,@LegacyPayrollNumber
				,@Nationality
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESRPersons]
			SET [ESRPersonId] = @ESRPersonId
				,[EffectiveStartDate] = @EffectiveStartDate
				,[EffectiveEndDate] = @EffectiveEndDate
				,[EmployeeNumber] = @EmployeeNumber
				,[Title] = @Title
				,[LastName] = @LastName
				,[FirstName] = @FirstName
				,[MiddleNames] = @MiddleNames
				,[MaidenName] = @MaidenName
				,[PreferredName] = @PreferredName
				,[PreviousLastName] = @PreviousLastName
				,[Gender] = @Gender
				,[DateOfBirth] = @DateOfBirth
				,[NINumber] = @NINumber
				,[NHSUniqueId] = @NHSUniqueId
				,[HireDate] = @HireDate
				,[ActualTerminationDate] = @ActualTerminationDate
				,[TerminationReason] = @TerminationReason
				,[EmployeeStatusFlag] = @EmployeeStatusFlag
				,[WTROptOut] = @WTROptOut
				,[WTROptOutDate] = @WTROptOutDate
				,[EthnicOrigin] = @EthnicOrigin
				,[MaritalStatus] = @MaritalStatus
				,[CountryOfBirth] = @CountryOfBirth
				,[PreviousEmployer] = @PreviousEmployer
				,[PreviousEmployerType] = @PreviousEmployerType
				,[CSD3Months] = @CSD3Months
				,[CSD12Months] = @CSD12Months
				,[NHSCRSUUID] = @NHSCRSUUID
				,[SystemPersonType] = @SystemPersonType
				,[UserPersonType] = @UserPersonType
				,[OfficeEmailAddress] = @OfficeEmailAddress
				,[NHSStartDate] = @NHSStartDate
				,[ESRLastUpdateDate] = @ESRLastUpdateDate
				,[DisabilityFlag] = @DisabilityFlag
				,[LegacyPayrollNumber] = @LegacyPayrollNumber
				,[Nationality] = @Nationality
			WHERE [ESRPersonId] = @ESRPersonId
		END

		SET @index = @index + 1
	END

	RETURN 0;
END