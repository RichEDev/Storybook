CREATE FUNCTION [dbo].[getProjectcodeSplit] (@expenseid int)  
RETURNS nvarchar (50) AS  
BEGIN 
declare @Projectcode nvarchar(50)
if (select count(distinct projectcodeid) from savedexpenses_costcodes where expenseid = @expenseid) > 1 

	set @Projectcode = 'Split'

else 

	set @Projectcode = (select Projectcode from project_codes where projectcodeid in (select projectcodeid from savedexpenses_costcodes where expenseid = @expenseid))

return @Projectcode
END

