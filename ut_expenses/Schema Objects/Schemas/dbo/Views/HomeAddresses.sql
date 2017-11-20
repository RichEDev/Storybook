CREATE VIEW [dbo].[HomeAddresses]
AS
SELECT DISTINCT EmployeeHomeAddresses.EmployeeHomeAddressId,
	EmployeeHomeAddresses.EmployeeId,
	EmployeeHomeAddresses.AddressId,
	CASE WHEN ESR.AddressId IS NULL THEN StartDate ELSE ESR.EffectiveStartDate END as StartDate,
	CASE WHEN ESR.AddressId IS NULL THEN EndDate ELSE ESR.EffectiveEndDate END as EndDate,
	EmployeeHomeAddresses.CreatedOn,
	EmployeeHomeAddresses.CreatedBy,
	EmployeeHomeAddresses.ModifiedOn,
	EmployeeHomeAddresses.ModifiedBy,
	ESR.ESRAddressID AS ESRAddressID,
	Addresses.Line1,
	Addresses.Postcode,
	Addresses.City
FROM EmployeeHomeAddresses
INNER JOIN Addresses ON EmployeeHomeAddresses.AddressId = Addresses.AddressId
LEFT JOIN (
	SELECT EmployeeHomeAddresses.EmployeeId,
		EmployeeHomeAddresses.AddressId,
		ESRAddresses.ESRAddressId,
		ESRAddresses.EffectiveStartDate,
		ESRAddresses.EffectiveEndDate
	FROM EmployeeHomeAddresses
	INNER JOIN employees ON employees.employeeid = EmployeeHomeAddresses.EmployeeID
	LEFT JOIN ESRAddresses ON ESRAddresses.ESRPersonId = employees.ESRPersonId
	INNER JOIN AddressEsrAllocation ON EmployeeHomeAddresses.AddressId = AddressEsrAllocation.AddressId AND ESRAddresses.ESRAddressId = AddressEsrAllocation.ESRAddressID
	) ESR ON esr.employeeID = EmployeeHomeAddresses.EmployeeId AND esr.AddressId = EmployeeHomeAddresses.AddressId