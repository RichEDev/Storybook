Create PROCEDURE [dbo].[GetApprovedAmountByChecker] 
@CheckerId int,
@ClaimantId int,
@StartDate DateTime,
@EndDate DateTime

As
BEGIN
SELECT SUM(ClaimAmount) as amount from ClaimApproverDetails where   checkerid = @CheckerId
 and ClaimantId = @ClaimantId and ( CreatedOn >= @StartDate and CreatedOn <= @EndDate ) 


END