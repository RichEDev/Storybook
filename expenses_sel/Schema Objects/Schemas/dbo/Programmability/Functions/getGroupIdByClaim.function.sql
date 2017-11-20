-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[getGroupIdByClaim] (@claimid int, @employeeid int) 
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	declare @groupid int;
	declare @groupidcc int;
	declare @groupidpc int;
	declare @cashcount int;
	declare @groupidrequired int;
	declare @creditcardcount int;
	declare @purchasecardcount int;
	-- Add the T-SQL statements to compute the return value here
	SELECT @groupid = groupid, @groupidcc = groupidcc, @groupidpc = groupidpc from employees where employeeid = @employeeid;
	set @cashcount = (select count(*) from savedexpenses where claimid = @claimid and itemtype = 1);
	set @creditcardcount = (select count(*) from savedexpenses where claimid = @claimid and itemtype = 2);
	set @purchasecardcount = (select count(*) from savedexpenses where claimid = @claimid and itemtype = 3);
	


	if (@creditcardcount > 0 and @purchasecardcount = 0)
		begin
			set @groupidrequired = @groupidcc;
		end

	if (@creditcardcount = 0 and @purchasecardcount > 0)
		begin
			set @groupidrequired = @groupidpc;
		end

	if @cashcount > 0 or (@creditcardcount = 0 and @purchasecardcount = 0) or (@creditcardcount > 0 and @purchasecardcount > 0) or (@groupidrequired is null)
		begin
			set @groupidrequired = @groupid
		end
	-- Return the result of the function
	
	return @groupidrequired;

END
