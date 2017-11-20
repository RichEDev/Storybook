CREATE PROCEDURE [dbo].[AllocateCostCodeTeamItemCheckers] (@claimId int, @checkerId int, @subAccountId int)
AS
BEGIN
 DECLARE @ownerType int;
 DECLARE @ownerId int;
 
 EXEC dbo.GetDefaultCostCodeOwner @subAccountId, @ownerType out, @ownerId out;

 UPDATE savedexpenses SET itemCheckerId = @checkerId WHERE claimid = @claimId and expenseid IN (SELECT expenseid FROM dbo.GetCostCodeOwnerExpenseIds(@claimId, @checkerId, 1, 0))

 if @ownerType = 49 -- Team
 begin
  UPDATE savedexpenses SET itemCheckerId = @checkerId WHERE claimid = @claimId AND expenseid IN 
  (
   SELECT savedexpenses_costcodes.expenseid FROM savedexpenses_costcodes
   inner join savedexpenses on savedexpenses_costcodes.expenseid = savedexpenses.expenseid 
   left join costcodes on costcodes.costcodeid = savedexpenses_costcodes.costcodeid
   where savedexpenses.claimid = @claimId 
   and 
   (
    savedexpenses_costcodes.costcodeid is null or 
    (
     savedexpenses_costcodes.costcodeid is not null and 
     costcodes.OwnerBudgetHolderId is null and costcodes.OwnerEmployeeId is null and costcodes.OwnerTeamId is null
    )
   )
  )
 end
END


GO