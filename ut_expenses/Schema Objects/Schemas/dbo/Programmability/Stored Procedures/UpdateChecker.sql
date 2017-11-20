Create PROCEDURE [dbo].[UpdateChecker]
	@CheckerId int,
	@ClaimId int

AS
BEGIN
	
		  UPDATE claims_base set checkerid=@CheckerId WHERE claimid=@ClaimId
	
return @checkerid
	END

