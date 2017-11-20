CREATE FUNCTION [dbo].[getBaseCurrency] ( @employeeid int)
RETURNS int AS  
BEGIN 

declare @basecurrency int

set @basecurrency = (select primarycurrency from employees where employeeid = @employeeid)

if (@basecurrency is null)
	begin
		set @basecurrency = (select basecurrency from [other])
	end


return @basecurrency
END
