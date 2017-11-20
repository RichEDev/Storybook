CREATE VIEW [dbo].[WorkAddresses]
AS
	SELECT
		EmployeeWorkAddresses.EmployeeWorkAddressId,
		EmployeeWorkAddresses.EmployeeID,
		EmployeeWorkAddresses.AddressID,
		EmployeeWorkAddresses.StartDate,
		EmployeeWorkAddresses.EndDate,
		EmployeeWorkAddresses.Active,
		EmployeeWorkAddresses.Temporary,
		EmployeeWorkAddresses.CreatedOn,
		EmployeeWorkAddresses.CreatedBy,
		EmployeeWorkAddresses.ModifiedOn,
		EmployeeWorkAddresses.ModifiedBy,
		ESRAssignmentLocations.ESRLocationId,
		Addresses.City,
		Addresses.Line1,
		Addresses.Postcode
	FROM
		EmployeeWorkAddresses
			INNER JOIN Addresses ON (EmployeeWorkAddresses.AddressId = Addresses.AddressId)
			left join ESRAssignmentLocations on ESRAssignmentLocations.ESRAssignmentLocationId = EmployeeWorkAddresses.ESRAssignmentLocationId
