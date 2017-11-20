CREATE PROCEDURE [dbo].[SaveFinancialYear] @EmployeeId INT
	,@DelegateId INT
	,@SubAccountId INT
	,@FinancialYearId INT
	,@Description NVARCHAR(100)
	,@YearStart DATETIME
	,@YearEnd DATETIME
	,@Active BIT
	,@Primary BIT
AS
IF @Primary = 1
BEGIN
	UPDATE FinancialYears
	SET [primary] = 0
END

IF @FinancialYearID = 0
BEGIN
	INSERT INTO financialyears (
		[description]
		,yearstart
		,yearend
		,active
		,[primary]
		)
	VALUES (
		@Description
		,@YearStart
		,@YearEnd
		,@Active
		,@Primary
		)

	SET @FinancialYearID = Scope_identity();

	EXEC addInsertEntryToAuditLog @EmployeeId
		,@DelegateId
		,184
		,@FinancialYearID
		,@Description
		,@SubAccountId;
END
ELSE
BEGIN
	DECLARE @OldDescription NVARCHAR(100)
	DECLARE @OldYearStart DATETIME
	DECLARE @OldYearEnd DATETIME
	DECLARE @OldActive BIT
	DECLARE @OldPrimary BIT

	SELECT @OldDescription = [description]
		,@OldYearStart = yearstart
		,@OldYearEnd = yearend
		,@OldActive = active
		,@OldPrimary = [primary]
	FROM FinancialYears
	WHERE FinancialYearID = @FinancialYearID

	UPDATE financialyears
	SET [description] = @Description
		,yearstart = @YearStart
		,yearend = @YearEnd
		,active = @Active
		,[primary] = @Primary
	WHERE FinancialYearID = @FinancialYearID

	IF (@OldDescription <> @Description)
		EXEC addUpdateEntryToAuditLog @EmployeeId
			,@DelegateID
			,184
			,@FinancialYearID
			,'EAEAB5BF-FF4D-435D-A049-99097943AFC4'
			,@OldDescription
			,@Description
			,@Description
			,@SubAccountID;

	IF (@OldYearStart <> @YearStart)
    
    DECLARE @OldshortDateStart NVARCHAR(5)
	DECLARE @ShortDateStart NVARCHAR(5)

	SET @OldshortDateStart = convert(NVARCHAR(2), DATEPART(dd, @OldYearStart)) + '/' + convert(NVARCHAR(2), DATEPART(mm, @OldYearStart))
	SET @ShortDateStart = convert(NVARCHAR(2), DATEPART(dd, @YearStart)) + '/' + convert(NVARCHAR(2), DATEPART(mm, @YearStart))

	EXEC addUpdateEntryToAuditLog @EmployeeId
		,@DelegateID
		,184
		,@FinancialYearID
		,'45780D82-A976-4322-8D09-BDB1E3A8788C'
		,@OldshortDateStart
		,@ShortDateStart
		,@Description
		,@SubAccountID;

	IF (@OldYearEnd <> @YearEnd)
		DECLARE @OldshortDateEnd NVARCHAR(5)
	DECLARE @shortDateEnd NVARCHAR(5)

	SET @OldshortDateEnd = convert(NVARCHAR(2), DATEPART(dd, @OldYearEnd)) + '/' + convert(NVARCHAR(2), DATEPART(mm, @OldYearEnd))
	SET @ShortDateEnd = convert(NVARCHAR(2), DATEPART(dd, @YearEnd)) + '/' + convert(NVARCHAR(2), DATEPART(mm, @YearEnd))

	PRINT @OldshortDateEnd

	EXEC addUpdateEntryToAuditLog @EmployeeId
		,@DelegateID
		,184
		,@FinancialYearID
		,'387B0BBF-308A-479A-8971-F00AD9C5244A'
		,@OldshortDateEnd
		,@ShortDateEnd
		,@Description
		,@SubAccountID;

	IF (@OldActive <> @Active)
		EXEC addUpdateEntryToAuditLog @EmployeeId
			,@DelegateID
			,184
			,@FinancialYearID
			,'C168AC28-FB23-4C82-A881-03D775154D9D'
			,@OldActive
			,@Active
			,@Description
			,@SubAccountID;

	IF (@OldPrimary <> @Primary)
		EXEC addUpdateEntryToAuditLog @EmployeeId
			,@DelegateID
			,184
			,@FinancialYearID
			,'2FB476F7-32A7-4F20-97BA-5AE656A27B02'
			,@OldPrimary
			,@Primary
			,@Description
			,@SubAccountID;
END

RETURN @FinancialYearID;