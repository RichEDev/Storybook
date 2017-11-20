CREATE FUNCTION [dbo].[getCostcodeSplit] (@expenseid int)  
RETURNS nvarchar (50) AS  
BEGIN 
declare @costcode nvarchar(50)
if (select count(distinct costcodeid) from savedexpenses_costcodes where expenseid = @expenseid) > 1 

	set @costcode = 'Split'

else 

	set @costcode = (select costcode from costcodes where costcodeid in (select costcodeid from savedexpenses_costcodes where expenseid = @expenseid))

return @costcode
END


