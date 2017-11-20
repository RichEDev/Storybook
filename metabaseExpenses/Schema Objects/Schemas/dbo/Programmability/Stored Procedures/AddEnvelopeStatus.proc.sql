CREATE PROCEDURE [dbo].[AddEnvelopeStatus]
	@message nvarchar(200)
AS
BEGIN
 INSERT INTO [dbo].[EnvelopeStatus] (
	Message 
) VALUES (
	@message
 )
 RETURN SCOPE_IDENTITY()
END