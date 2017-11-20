CREATE PROCEDURE [dbo].[SaveEnvelopePhysicalState]
	@id int,
	@details nvarchar(100)
AS
BEGIN
	IF (@id = 0)
	BEGIN
		IF ((SELECT COUNT (EnvelopePhysicalState.Details) FROM EnvelopePhysicalState WHERE Details = @details) > 0) RETURN -1

		INSERT INTO [dbo].[EnvelopePhysicalState] (Details)
		VALUES (@details)
		RETURN SCOPE_IDENTITY()
	END
	ELSE 
	BEGIN
		IF ((SELECT COUNT (EnvelopePhysicalState.EnvelopePhysicalStateId) FROM EnvelopePhysicalState WHERE EnvelopePhysicalStateId = @id) = 0) RETURN -2
		
		UPDATE [dbo].[EnvelopePhysicalState]
		SET Details = @details
		WHERE EnvelopePhysicalStateId = @id
		RETURN @id
	END	
END