CREATE FUNCTION [dbo].[claimType] (@claimid int)  
RETURNS int AS  
BEGIN 
declare @itemcount int
declare @cardcount int
declare @creditcount int
declare @returncode int
set @itemcount = (select count(*) from savedexpenses where claimid = @claimid)
set @cardcount = (select count(*) from savedexpenses where claimid = @claimid and itemtype = 1)
if @itemcount = @cardcount
	begin
		set @returncode = 1
	end
else
	begin
		set @cardcount = (select count(*) from savedexpenses where claimid = @claimid and itemtype = 2)
		if @cardcount = @itemcount
			set @returncode = 2
		else
			set @returncode = 0
		
	end
return @returncode
END
