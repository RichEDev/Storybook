CREATE PROCEDURE [dbo].[APIGetEmployeeLocations]

AS
	select distinct employees.employeeid,postcode from employees
	inner join HomeAddresses on HomeAddresses.EmployeeId = employees.employeeid
	where Postcode > '' and ISNULL(employees.ExcessMileage,0) > 0
	union
	select distinct employees.employeeid,postcode from employees
	inner join WorkAddresses on WorkAddresses.EmployeeId = employees.employeeid
	where Postcode > '' and ISNULL(employees.ExcessMileage,0) > 0
