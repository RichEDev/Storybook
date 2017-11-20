CREATE PROCEDURE [dbo].[APIdeleteEmployeeWorkLocation]
	@employeeLocationID int
	
AS
	DELETE FROM employeeWorkLocations WHERE [employeeLocationID] = @employeeLocationID
RETURN 0