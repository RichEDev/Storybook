CREATE FUNCTION [dbo].[CostCodeOwnerTeamToCheck] (@claimId int)
returns bit
begin
	declare @IsCCOTeamCheck bit = 0;
	declare @count int = 0;	
	
	select @count = count(claims_base.claimid) from claims_base 
	inner join employees on claims_base.employeeid = employees.employeeid
	inner join signoffs on employees.groupid = signoffs.groupid and claims_base.stage = signoffs.stage and signofftype = 8
	inner join savedexpenses on claims_base.claimid = savedexpenses.claimid
	inner join savedexpenses_costcodes on savedexpenses.expenseid = savedexpenses_costcodes.expenseid
	inner join costcodes on savedexpenses_costcodes.costcodeid = costcodes.costcodeid
	where claims_base.claimid = @claimid and OwnerTeamId is not null

	if(@count > 0)
	begin
		set @IsCCOTeamCheck = 1;
	end
	
	return @IsCCOTeamCheck;
end