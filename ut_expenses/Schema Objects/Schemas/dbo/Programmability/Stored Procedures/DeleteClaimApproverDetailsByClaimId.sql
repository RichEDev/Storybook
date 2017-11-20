Create  PROCEDURE [dbo].[DeleteClaimApproverDetailsByClaimId] 
@ClaimId int
As
BEGIN
begin
delete from  ClaimApproverDetails where  ClaimId = @ClaimId
end
return @@ROWCOUNT
end