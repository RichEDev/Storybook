CREATE PROCEDURE [dbo].[UnlinkEnvelopeFromClaim]
	@envelopeId int,
	@lastModifiedBy int
AS
BEGIN
	SET CONCAT_NULL_YIELDS_NULL OFF;
	UPDATE [dbo].[Envelopes]
	SET 	
		ClaimId = NULL, 
		CRN = null,
		EnvelopeStatus = 20, 
		DateAssignedToClaim = NULL, 
		LastModifiedBy = @lastModifiedBy
	WHERE EnvelopeId = @envelopeId;
	SET CONCAT_NULL_YIELDS_NULL ON;
END