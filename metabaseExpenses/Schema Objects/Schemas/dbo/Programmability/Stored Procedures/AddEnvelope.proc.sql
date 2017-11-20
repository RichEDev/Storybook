CREATE PROCEDURE [dbo].[AddEnvelope]
	@accountId int = null,
	@claimId int = null,
	@envelopeNumber nvarchar(10),
	@crn nvarchar(12) = null,
	@envelopeStatus tinyint,
	@envelopeType int,
	@dateIssuedToClaimant DateTime = null,
	@dateAssignedToClaim DateTime = null,
	@dateReceived DateTime = null,
	@dateAttachCompleted DateTime = null,
	@dateDestroyed DateTime = null,
	@overpaymentCharge decimal(16,2) = null,
	@physicalStateProofUrl nvarchar(100) = null,
	@lastModifiedBy int,
	@declaredLostInPost bit
AS
BEGIN
 INSERT INTO [dbo].[Envelopes] (
	AccountId,
	ClaimId, 
	EnvelopeNumber,
	CRN,
	EnvelopeStatus,
	EnvelopeType,
	DateIssuedToClaimant,
	DateAssignedToClaim,
	DateReceived,
	DateAttachCompleted,
	DateDestroyed,
	OverpaymentCharge,
	PhysicalStateProofUrl,
	LastModifiedBy,
	DeclaredLostInPost
) VALUES (
	@accountId,
	@claimId,
	@envelopeNumber,
	@crn,
	@envelopeStatus,
	@envelopeType,
	@dateIssuedToClaimant,
	@dateAssignedToClaim,
	@dateReceived,
	@dateAttachCompleted,
	@dateDestroyed,
	@overpaymentCharge,
	@physicalStateProofUrl,
	@lastModifiedBy,
	@declaredLostInPost
 )
 RETURN SCOPE_IDENTITY()
END