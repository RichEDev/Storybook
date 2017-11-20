CREATE FUNCTION [dbo].[getPercentage] (@total float, @percentage int)  
RETURNS money AS  
BEGIN 

declare @newtotal money
	if @percentage is null
		set @newtotal =  @total
	
	else
		set @newtotal =  @total / 100 * @percentage

return @newtotal
END

