CREATE PROCEDURE GetEmployeeActiveJourneys 
@employeeId INT
AS
BEGIN
	SELECT JourneyID
		,SubcatID
		,JourneyJSON
		,CreatedBy
		,JourneyDate
		,JourneyEndTime
		,JourneyStartTime
	FROM MobileJourneys
	WHERE EmployeeID = @employeeId
	AND Active = 1
END
