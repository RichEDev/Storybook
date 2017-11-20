CREATE PROCEDURE [dbo].[APIdeleteEmployeeHomeAddress]
	@EmployeeHomeAddressId int
	
AS
	DELETE FROM [employeeHomeAddresses] WHERE [EmployeeHomeAddressId] = @EmployeeHomeAddressId
RETURN 0