CREATE PROCEDURE [dbo].[CountValidatableExpensesInClaimWithNoReceipt]
 @claimId int
AS
DECLARE @count INT;
SELECT @count = COUNT(expenseid) FROM savedexpenses where receipt = 1 AND receiptattached = 0 AND ValidationProgress = -4 AND claimid = @claimId

RETURN @count;
GO
