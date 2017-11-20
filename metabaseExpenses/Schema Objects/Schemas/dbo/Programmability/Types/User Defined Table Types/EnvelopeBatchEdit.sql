CREATE TYPE dbo.EnvelopeBatchEdit AS TABLE 
(
	EnvelopeId int NOT NULL,
	AccountId int NULL,
	ClaimId int NULL, 
	EnvelopeNumber nvarchar(10) NULL,
	CRN nvarchar(12) NULL,
	EnvelopeStatus tinyint NULL,
	EnvelopeType int NULL,
	DateIssuedToClaimant DateTime NULL,
	DateAssignedToClaim DateTime NULL,
	DateReceived DateTime NULL,
	DateAttachCompleted DateTime NULL,
	DateDestroyed DateTime NULL,
	OverpaymentCharge decimal(16,2) NULL,
	PhysicalStateProofUrl nvarchar(100) NULL,
	LastModifiedBy int NULL,
	DeclaredLostInPost bit
)