CREATE PROCEDURE [dbo].[GetEmployeesByLocation]
	@locationIdentity INT,
	@homeLocations BIT
AS
BEGIN
	IF @homeLocations = 1
	BEGIN
		SELECT DISTINCT [employeeID] FROM [dbo].[employeeHomeLocations] WHERE [locationID] = @locationIdentity;
	END
	ELSE
	BEGIN
		SELECT DISTINCT [employeeID] FROM [dbo].[employeeWorkLocations] WHERE [locationID] = @locationIdentity;
	END
END