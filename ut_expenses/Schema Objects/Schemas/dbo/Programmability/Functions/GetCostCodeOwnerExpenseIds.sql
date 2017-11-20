CREATE function [dbo].[GetCostCodeOwnerExpenseIds] (@claimId int, @checkerEmployeeId int, @teamCheck bit, @checkerAssigned bit)
returns @outTable table (expenseid int)
begin
 declare @isCostCodeOwner bit = 0;
 declare @DefaultIsTeam int = (select TOP 1 case when charindex(',', stringvalue) is null then 0 else CAST(SUBSTRING(stringvalue,1,charindex(',', stringvalue)-1) as int) end  from accountProperties where stringKey = 'defaultCostCodeOwner');
 insert into @outTable
 select savedexpenses_costcodes.expenseid from savedexpenses_costcodes
 inner join savedexpenses on savedexpenses.expenseid = savedexpenses_costcodes.expenseid
 inner join claims_base on savedexpenses.claimid = claims_base.claimid
 left join costcodes on savedexpenses_costcodes.costcodeid = costcodes.costcodeid

 where claims_base.claimid = @claimid and 
 (
  (@checkerAssigned = 1 and savedexpenses.itemCheckerId is not null and savedexpenses.itemCheckerId = @checkerEmployeeId and (costcodes.OwnerTeamId is not null or (@DefaultIsTeam = 49 and costcodes.OwnerBudgetHolderId is null and costcodes.OwnerEmployeeId is null and costcodes.OwnerTeamId is null) )) 
  or 
  (
   (
    @checkerAssigned = 0 and savedexpenses.itemCheckerId is null
    and
    (
     (@teamCheck = 0 and
      (
      OwnerEmployeeId = @checkerEmployeeId
      or
      OwnerBudgetHolderId in (select budgetholderid from budgetholders where employeeid = @checkerEmployeeId)
      )
     )
     or
     (@teamCheck = 1 and
      (
      OwnerTeamId in (select teamid from dbo.TeamIdsMemberOf(@checkerEmployeeId))
      )
     )
    )
   )
  )
 )
 return;
end


GO