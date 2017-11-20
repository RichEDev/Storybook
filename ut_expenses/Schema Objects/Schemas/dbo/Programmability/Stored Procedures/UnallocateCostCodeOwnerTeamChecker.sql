CREATE PROCEDURE [dbo].[UnallocateCostCodeOwnerTeamChecker] (@claimId int, @checkerId int)
AS
BEGIN
 UPDATE savedexpenses SET itemCheckerId = NULL WHERE claimid = @claimId AND expenseid IN (SELECT expenseid FROM dbo.GetCostCodeOwnerExpenseIds(@claimId, @checkerId, 1, 1))
END


GO
