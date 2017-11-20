CREATE FUNCTION [dbo].[getEmployeeJobTitleFromEmployeeid] 
(@employeeId int)
RETURNS VARCHAR(200)
AS
BEGIN
DECLARE @jobtitle varchar(200);
SELECT TOP 1 @jobtitle = esr_assignments.PositionName FROM employees
inner join esr_assignments ON employees.employeeid = esr_assignments.employeeid
WHERE employees.employeeid = @employeeId 
ORDER BY esr_assignments.EffectiveStartDate DESC; 
return @jobtitle;
END