CREATE PROCEDURE [dbo].[DeleteEnvelopePhysicalState]
	@id int	
AS
BEGIN
	IF ((SELECT COUNT (EnvelopesPhysicalStates.EnvelopePhysicalStateId) FROM EnvelopesPhysicalStates WHERE EnvelopePhysicalStateId = @id) > 0) RETURN -1
	DELETE FROM EnvelopePhysicalState WHERE EnvelopePhysicalStateId = @id
END