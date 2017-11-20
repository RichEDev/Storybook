CREATE procedure [dbo].[ClaimHasItemChecker] 
 @claimId int
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    declare @hasItemChecker bit
    if (select count(expenseid) from savedexpenses where claimid = @claimId and itemcheckerid is not null) = 0
  set @hasItemChecker = 0
 else
  set @hasItemChecker = 1
  
 return @hasItemChecker
END
