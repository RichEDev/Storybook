CREATE PROCEDURE [dbo].[APIdeleteEmployeeHomeLocation]
	@employeeLocationID int
	
AS
	DELETE FROM employeeHomeLocations WHERE [employeeLocationID] = @employeeLocationID
RETURN 0