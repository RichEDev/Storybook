CREATE PROCEDURE [dbo].[APIgetEmployeeWorkAddresses]
	@EmployeeWorkAddressId INT
AS
	SELECT [EmployeeWorkAddressId]
		,[EmployeeId]
		,[AddressId]
		,[StartDate]
		,[EndDate]
		,[Active]
		,[Temporary]
		,[CreatedOn]
		,[CreatedBy]
		,[ModifiedOn]
		,[ModifiedBy]
		,[ESRAssignmentLocationId]
	FROM [dbo].[EmployeeWorkAddresses]
	WHERE [EmployeeWorkAddressId] = @EmployeeWorkAddressId
		or @EmployeeWorkAddressId = 0

	RETURN 0
