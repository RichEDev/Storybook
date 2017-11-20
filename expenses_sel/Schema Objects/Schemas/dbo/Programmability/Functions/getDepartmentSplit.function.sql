CREATE FUNCTION [dbo].[getDepartmentSplit] (@expenseid int)  
RETURNS nvarchar (50) AS  
BEGIN 
declare @department nvarchar(50)
if (select count(distinct departmentid) from savedexpenses_costcodes where expenseid = @expenseid) > 1 

	set @department = 'Split'

else 

	set @department = (select department from departments where departmentid in (select departmentid from savedexpenses_costcodes where expenseid = @expenseid))

return @department
END



