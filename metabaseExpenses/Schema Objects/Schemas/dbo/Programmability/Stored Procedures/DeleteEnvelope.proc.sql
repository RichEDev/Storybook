CREATE PROCEDURE [dbo].[DeleteEnvelope]
	@evenlopeId int,
	@lastModifiedBy int
AS
BEGIN
	DELETE FROM [dbo].[Envelopes]
	WHERE EnvelopeId = @evenlopeId;
END