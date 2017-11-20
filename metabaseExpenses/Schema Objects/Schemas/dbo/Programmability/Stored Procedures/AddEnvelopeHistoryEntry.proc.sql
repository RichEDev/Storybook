CREATE PROCEDURE [dbo].[AddEnvelopeHistoryEntry]
	@envelopeId int,
	@envelopeStatus int,
	@data nvarchar(500),
	@lastModifiedBy int
AS
BEGIN
	INSERT INTO [dbo].[EnvelopeHistory]
	(
		EnvelopeId, 
		EnvelopeStatus,
		Data,
		ModifiedBy,
		ModifiedOn			
	) VALUES (
		@envelopeId,
		@envelopeStatus,
		@data,
		@lastModifiedBy,
		GETUTCDATE()
	)
END