CREATE FUNCTION [dbo].[getEmpCostcodeSplit] (@employeeid int)  
RETURNS nvarchar (50) AS  
BEGIN 
declare @costcode nvarchar(50)
if (select count(distinct costcodeid) from employee_costcodes where employeeid = @employeeid) > 1 

	set @costcode = 'Split'

else 

	set @costcode = (select costcode from costcodes where costcodeid in (select costcodeid from employee_costcodes where employeeid = @employeeid))

return @costcode
END




