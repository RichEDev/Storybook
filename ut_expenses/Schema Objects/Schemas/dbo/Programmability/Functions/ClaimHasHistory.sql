CREATE function [dbo].[ClaimHasHistory]
(
 @claimId int
)
RETURNS bit
AS
BEGIN
 
 DECLARE @hasHistory bit
 
 -- Add the T-SQL statements to compute the return value here
 if (select count(claimid) from claimhistory where claimid = @claimId) = 0
  set @hasHistory = 0
 else
  set @hasHistory = 1
 
 return @hasHistory

END
