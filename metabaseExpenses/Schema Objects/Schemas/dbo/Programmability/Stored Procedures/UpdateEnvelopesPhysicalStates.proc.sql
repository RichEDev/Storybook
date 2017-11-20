CREATE PROCEDURE [dbo].[UpdateEnvelopesPhysicalStates]
	@envelopeId int,
	@physicalStateIds IntPK readonly
AS
BEGIN
	DELETE FROM EnvelopesPhysicalStates WHERE EnvelopeId = @envelopeId AND EnvelopePhysicalStateId NOT IN (SELECT c1 FROM @physicalStateIds)
	INSERT INTO EnvelopesPhysicalStates (EnvelopeId, EnvelopePhysicalStateId)
	SELECT @envelopeId, c1 FROM @physicalStateIds 
		WHERE (c1 IN (SELECT EnvelopePhysicalStateId FROM EnvelopePhysicalState) 
				AND c1 NOT IN (SELECT EnvelopePhysicalStateId FROM EnvelopesPhysicalStates WHERE EnvelopeId = @envelopeId))
	return @envelopeId
END
GO