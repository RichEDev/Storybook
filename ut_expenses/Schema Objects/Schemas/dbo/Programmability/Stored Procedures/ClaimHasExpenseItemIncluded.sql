CREATE PROCEDURE ClaimHasExpenseItemIncluded 
 @claimId int,
 @subcatId int
AS
BEGIN
 
 SET NOCOUNT ON;

    declare @hasItems bit
 if (select count(savedexpenses.expenseid) from savedexpenses where savedexpenses.claimid = @claimId and savedexpenses.subcatid = @subcatId) = 0
  set @hasItems = 0
 else
  set @hasItems = 1
  
 return @hasItems
END