
CREATE VIEW [dbo].[WorkLocations]
	AS SELECT DISTINCT    employeeWorkLocations.employeeLocationID, employeeWorkLocations.employeeID, employeeWorkLocations.locationID, employeeWorkLocations.startDate, 
                      employeeWorkLocations.endDate, employeeWorkLocations.active, employeeWorkLocations.temporary, employeeWorkLocations.createdOn, 
                      employeeWorkLocations.createdBy, employeeWorkLocations.modifiedOn, employeeWorkLocations.modifiedBy, ESR.ESRLocationId AS ESRLocationID,
					  companies.company, companies.city, companies.address1, companies.postcode
FROM         employeeWorkLocations 
INNER JOIN 
			companies on (employeeWorkLocations.locationID = companies.companyid )
LEFT JOIN (SELECT employeeWorkLocations.employeeID, employeeWorkLocations.locationID, esr_assignments.ESRLocationId AS ESRLocationID, employeeWorkLocations.startDate
FROM employeeWorkLocations  
LEFT JOIN esr_assignments ON employeeWorkLocations.employeeID = esr_assignments.employeeid and employeeWorkLocations.startDate = esr_assignments.EffectiveStartDate
INNER JOIN CompanyEsrAllocation ON employeeWorkLocations.locationID = CompanyEsrAllocation.companyid AND esr_assignments.ESRLocationId = CompanyEsrAllocation.ESRLocationID )
ESR on employeeWorkLocations.locationID = esr.locationID and employeeWorkLocations.employeeID = esr.employeeID and employeeWorkLocations.startDate = esr.startDate
