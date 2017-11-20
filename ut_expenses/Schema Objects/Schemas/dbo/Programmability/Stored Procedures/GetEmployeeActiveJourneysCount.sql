CREATE PROCEDURE [dbo].GetEmployeeActiveJourneysCount @employeeId INT
AS
BEGIN
	SELECT Count(JourneyID)
	FROM MobileJourneys
	WHERE EmployeeID = @employeeId
      AND Active = 1
END