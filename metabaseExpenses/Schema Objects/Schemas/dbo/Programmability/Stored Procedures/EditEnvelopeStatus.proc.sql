CREATE PROCEDURE [dbo].[EditEnvelopeStatus]
	@id int,
	@message nvarchar(200)
AS
BEGIN
 UPDATE [dbo].[EnvelopeStatus]
	SET Message = @message
	WHERE EnvelopeStatusId = @id;
END