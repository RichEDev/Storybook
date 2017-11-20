CREATE PROCEDURE [dbo].[SaveMobileJourney] @employeeID INT
	,@deviceTypeID INT
	,@mileageJson NVARCHAR(max)
	,@journeyDate DATETIME
	,@startTime DATETIME
	,@endTime DATETIME
	,@createdBy INT
	,@subcatID INT
	,@active BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @identity INT;

	IF NOT EXISTS (
			SELECT item_roles.itemroleid
			FROM employee_roles
			INNER JOIN item_roles ON item_roles.itemroleid = employee_roles.itemroleid
			INNER JOIN rolesubcats ON rolesubcats.roleid = item_roles.itemroleid
			WHERE employee_roles.employeeid = @employeeID
				AND rolesubcats.subcatid = @subcatID
			)
	BEGIN
		RETURN - 7;-- MobileReturnCode for SubcatIsInvalid
	END

	INSERT INTO MobileJourneys (
		EmployeeID
		,SubcatID
		,DeviceTypeID
		,JourneyJSON
		,JourneyDate
		,CreatedBy
		,CreatedOn
		,JourneyStartTime
		,JourneyEndTime
		,Active
		)
	VALUES (
		@employeeID
		,@subcatID
		,@deviceTypeID
		,@mileageJson
		,@journeyDate
		,@createdBy
		,GETUTCDATE()
		,@startTime
		,@endTime
		,@active
		)

	SET @identity = SCOPE_IDENTITY();

	RETURN @identity

END