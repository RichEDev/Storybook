CREATE PROCEDURE [dbo].[ClaimHasCostcodeIncluded] 
 @claimId int,
 @costcodeId int
AS
BEGIN
 
 SET NOCOUNT ON;

    declare @hasItems bit
 if (select count(savedexpenses_costcodes.costcodeid) from savedexpenses_costcodes inner join savedexpenses on savedexpenses.expenseid = savedexpenses_costcodes.expenseid where savedexpenses.claimid = @claimId and savedexpenses_costcodes.costcodeid = @costcodeId) = 0
  set @hasItems = 0
 else
  set @hasItems = 1
  
 return @hasItems
END