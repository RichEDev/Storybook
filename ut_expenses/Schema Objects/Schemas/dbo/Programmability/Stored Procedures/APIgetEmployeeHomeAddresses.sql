CREATE PROCEDURE [dbo].[APIgetEmployeeHomeAddresses]
	@EmployeeHomeAddressId INT
AS
BEGIN
	IF @EmployeeHomeAddressId = 0
		BEGIN
			SELECT [EmployeeHomeAddressId]
			  ,[EmployeeId]
			  ,[AddressId]
			  ,[StartDate]
			  ,[EndDate]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeHomeAddresses]
		END
	ELSE
		BEGIN
			SELECT [EmployeeHomeAddressId]
			  ,[EmployeeId]
			  ,[AddressId]
			  ,[StartDate]
			  ,[EndDate]
			  ,[CreatedOn]
			  ,[CreatedBy]
			  ,[ModifiedOn]
			  ,[ModifiedBy]
		  FROM [dbo].[employeeHomeAddresses]
		  WHERE [EmployeeHomeAddressId] = @EmployeeHomeAddressId
		END
END
RETURN 0