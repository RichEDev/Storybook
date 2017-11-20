Create PROCEDURE [dbo].[ShowUnallocateIcon]
 @ClaimId int
AS

BEGIN
 declare @ItemCount int 
 set @ItemCount=(select Count(*)  from savedexpenses where claimid=@ClaimId and (IsItemEscalated =0 or IsItemEscalated =1))
 if @ItemCount>=1
 begin
 select cast(1 as bit)
 end
Else
 begin
 select cast(0 as bit)
 end
END