CREATE PROCEDURE [dbo].[DeleteEnvelopeType]
	@id int
AS
BEGIN
	DELETE FROM [dbo].[EnvelopeTypes]
	WHERE EnvelopeTypeId = @id;
END