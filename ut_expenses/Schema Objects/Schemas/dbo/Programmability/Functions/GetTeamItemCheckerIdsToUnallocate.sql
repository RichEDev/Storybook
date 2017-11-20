CREATE FUNCTION [dbo].[GetTeamItemCheckerIdsToUnallocate] (@claimId INT, @checkerId INT)
RETURNS @outTable TABLE (expenseid INT)
AS
BEGIN
 DECLARE @defaultOwnerType INT;
 DECLARE @defaultOwnerId INT;
 
 declare @defaultProperty nvarchar(20);
 select top 1 @defaultProperty = stringValue from accountProperties where stringKey = 'defaultCostCodeOwner';

 if @defaultProperty is not null and len(@defaultProperty) > 0
 begin
  declare @separatorIdx int = (CHARINDEX(',',@defaultProperty));
  if @separatorIdx > 0
  begin
   set @defaultOwnerType = CAST(SUBSTRING(@defaultProperty,1,@separatorIdx-1) as int);
   set @defaultOwnerId = CAST(SUBSTRING(@defaultProperty, @separatorIdx+1, LEN(@defaultProperty) - @separatorIdx) AS INT);
  end
 end

 INSERT INTO @outTable
 SELECT savedexpenses.expenseid FROM savedexpenses 
 INNER JOIN savedexpenses_costcodes ON savedexpenses.expenseid = savedexpenses_costcodes.expenseid
 LEFT JOIN costcodes ON savedexpenses_costcodes.costcodeid = costcodes.costcodeid
 WHERE 
 claimid = @claimId AND 
 itemCheckerId IS NOT NULL AND itemCheckerId = @checkerId
 AND
 (
  (
   costcodes.OwnerTeamId IS NOT NULL AND costcodes.OwnerTeamId IN (SELECT teamid FROM dbo.TeamIdsMemberOf(@checkerId))
  )
  OR
  (
   costcodes.OwnerBudgetHolderId IS NULL AND costcodes.OwnerEmployeeId IS NULL AND costcodes.OwnerTeamId IS NULL AND @defaultOwnerType = 49
   AND
   @defaultOwnerId IN (SELECT teamid FROM dbo.TeamIdsMemberOf(@checkerId))
  )
 )
 
 RETURN
END


GO