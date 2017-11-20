CREATE FUNCTION [dbo].[getEmployeeDepartmentNameFromEmployeeId] 
(@employeeId int)
RETURNS VARCHAR(200)
AS
BEGIN
 declare @departmentName varchar(200);
 select top 1 @departmentName = ESROrganisations.OrganisationName from employees
 inner join esr_assignments on employees.employeeid = esr_assignments.employeeid
 inner join ESROrganisations on esr_assignments.ESROrganisationId = ESROrganisations.ESROrganisationId
 where employees.employeeid = @employeeId; 

 return @departmentname; 

END