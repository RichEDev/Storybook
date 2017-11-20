create function dbo.ClaimHasNullCostCodeItems (@claimId int)
returns int
begin
	declare @CCOItems int = 0;
	
	select @CCOItems = count(savedcostcodeid) from savedexpenses_costcodes 
	left join costcodes on savedexpenses_costcodes.costcodeid = costcodes.costcodeid
	inner join savedexpenses on savedexpenses_costcodes.expenseid = savedexpenses.expenseid
	where savedexpenses.claimid = @claimId
	and 
	(
		savedexpenses_costcodes.costcodeid is null 
		or 
		(
			savedexpenses_costcodes.costcodeid is not null 
			and 
			(
				costcodes.OwnerBudgetHolderId is null and costcodes.OwnerEmployeeId is null and costcodes.OwnerTeamId is null
			)
		)
	)

	return @CCOItems
end