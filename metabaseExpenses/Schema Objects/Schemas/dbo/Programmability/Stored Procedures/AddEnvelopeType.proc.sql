CREATE PROCEDURE [dbo].[AddEnvelopeType]
	@label nvarchar(60)
AS
BEGIN
 INSERT INTO [dbo].[EnvelopeTypes] (
	Label
) VALUES (
	@label
 )
 RETURN SCOPE_IDENTITY()
END