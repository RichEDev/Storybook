CREATE FUNCTION [dbo].[getFinancialYear](@date DateTime ) 
RETURNS int
AS
BEGIN

declare @newyear DateTime;

set @newyear =  (Convert(DateTime, CAST(datepart(yyyy,@date) as nvarchar) + '-04-06',120))
	-- Declare the return variable here
	if @date < @newyear
		return datepart(yyyy,@date) - 1;

return datepart(yyyy,@date)
END
