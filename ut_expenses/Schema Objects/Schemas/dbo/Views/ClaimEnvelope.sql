CREATE VIEW [dbo].[ClaimEnvelope]
AS
SELECT        EnvelopeId AS ClaimEnvelopeId, ClaimId, EnvelopeNumber
FROM            [$(targetMetabase)].dbo.Envelopes
 
GO