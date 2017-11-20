CREATE PROCEDURE [dbo].[APIdeleteEmployeeWorkAddress]
	@EmployeeWorkAddressId int
	
AS
	DELETE FROM EmployeeWorkAddresses WHERE [EmployeeWorkAddressId] = @EmployeeWorkAddressId;
	RETURN 0