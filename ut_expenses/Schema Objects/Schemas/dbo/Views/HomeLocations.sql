CREATE VIEW [dbo].[HomeLocations]
	AS  
SELECT distinct employeeHomeLocations.employeeLocationID, employeeHomeLocations.employeeID, employeeHomeLocations.locationID, employeeHomeLocations.startDate, 
                      employeeHomeLocations.endDate,  employeeHomeLocations.createdOn, 
                      employeeHomeLocations.createdBy, employeeHomeLocations.modifiedOn, employeeHomeLocations.modifiedBy, ESR.ESRAddressID AS ESRAddressID,
					  companies.company, companies.address1, companies.postcode, companies.city
FROM         employeeHomeLocations 
INNER JOIN companies ON employeeHomeLocations.locationID = companies.companyid
LEFT JOIN ( SELECT     employeeHomeLocations.employeeID, employeeHomeLocations.locationID, ESRAddresses.ESRAddressId 
	FROM employeeHomeLocations  
	INNER JOIN employees on employees.employeeid = employeeHomeLocations.employeeID
	LEFT JOIN ESRAddresses on ESRAddresses.ESRPersonId = employees.ESRPersonId 
	INNER JOIN CompanyEsrAllocation ON employeeHomeLocations.locationID = CompanyEsrAllocation.companyid and ESRAddresses.ESRAddressId = CompanyEsrAllocation.ESRAddressID) ESR 
on esr.employeeID = employeeHomeLocations.employeeID and esr.locationID = employeeHomeLocations.locationID