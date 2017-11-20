CREATE FUNCTION [dbo].[getEmpDepartmentSplit] (@employeeid int)  
RETURNS nvarchar (50) AS  
BEGIN 
declare @department nvarchar(50)
if (select count(distinct departmentid) from employee_costcodes where employeeid = @employeeid) > 1 

	set @department = 'Split'

else 

	set @department = (select department from departments where departmentid in (select departmentid from employee_costcodes where employeeid = @employeeid))

return @department
END





