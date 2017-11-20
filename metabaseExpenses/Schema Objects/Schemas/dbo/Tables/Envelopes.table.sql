CREATE TABLE [Envelopes] 
(
   EnvelopeId int NOT NULL constraint PK_Envelopes Primary Key IDENTITY(1,1),
   AccountId int constraint FK_Envelopes_RegisteredUsers Foreign Key REFERENCES dbo.registeredusers NULL,
   ClaimId int NULL, 
   EnvelopeNumber nvarchar(10) NOT NULL,
   CRN nvarchar(12) NULL,
   EnvelopeStatus tinyint NOT NULL,
   EnvelopeType int constraint FK_Envelopes_EnvelopeTypes Foreign Key REFERENCES dbo.EnvelopeTypes NULL,
   DateIssuedToClaimant DateTime NULL,
   DateAssignedToClaim DateTime NULL,
   DateReceived DateTime NULL,
   DateAttachCompleted DateTime NULL,
   DateDestroyed DateTime NULL,
   OverpaymentCharge decimal(16,2) NULL,
   PhysicalState tinyint NOT NULL,
   PhysicalStateProofUrl nvarchar(100) NULL,
   LastModifiedBy int NOT NULL,
   DeclaredLostInPost bit NOT NULL DEFAULT(0)
)
GO
create index Index_Envelopes_EnvelopeNumber on [dbo].[Envelopes](EnvelopeNumber)
GO
create index Index_Envelopes_EnvelopeCRN_FreeText on [dbo].[Envelopes](CRN)
