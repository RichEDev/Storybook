-- delete envelope status
CREATE PROCEDURE [dbo].[DeleteEnvelopeStatus]
	@id int
AS
BEGIN
	DELETE FROM [dbo].[EnvelopeStatus]
	WHERE EnvelopeStatusId = @id;
END