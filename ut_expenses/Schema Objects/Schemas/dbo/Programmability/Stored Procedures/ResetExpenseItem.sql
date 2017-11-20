CREATE PROCEDURE [dbo].[ResetExpenseItem] 
 @ClaimId int
AS
BEGIN
 update savedexpenses set tempallow = 0, itemCheckerId = null, IsItemEscalated=NUll where claimid = @claimid
END