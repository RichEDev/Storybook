CREATE PROCEDURE [dbo].[AddEnvelopeBatch]
	@envelopes [EnvelopeBatchAdd] readonly
AS
BEGIN

-- create temp table
DECLARE @output TABLE
(
	Id int,
	EnvelopeNumber varchar(10)
)

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
) 
OUTPUT SCOPE_IDENTITY(), inserted.EnvelopeNumber
INTO @output
SELECT 
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
FROM @envelopes;

RETURN SELECT * FROM @output;
END