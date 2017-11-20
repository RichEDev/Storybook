CREATE PROCEDURE [dbo].[GetEsrAssignmentLocationsByEsrLocationIdAndEmployeeId] 
      @employeeId INT
	,@esrLocationId BIGINT
AS
BEGIN
	SELECT dbo.ESRAssignmentLocations.ESRAssignmentLocationId
	FROM dbo.ESRAssignmentLocations
	LEFT OUTER JOIN dbo.EmployeeWorkAddresses ON dbo.ESRAssignmentLocations.ESRAssignmentLocationId = dbo.EmployeeWorkAddresses.ESRAssignmentLocationId
	WHERE (dbo.ESRAssignmentLocations.ESRLocationId = @esrLocationId)
		AND (dbo.EmployeeWorkAddresses.EmployeeId = @employeeId)
END