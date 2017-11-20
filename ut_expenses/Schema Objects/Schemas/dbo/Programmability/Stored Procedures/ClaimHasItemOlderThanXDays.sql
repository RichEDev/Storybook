CREATE PROCEDURE [dbo].[ClaimHasItemOlderThanXDays]
 @claimId int,
 @date datetime
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    declare @hasItems bit
 if (select count(savedexpenses.expenseid) from savedexpenses where claimid = @claimId and date < @date) = 0
  set @hasItems = 0
 else
  set @hasItems = 1
 
 return @hasItems
END