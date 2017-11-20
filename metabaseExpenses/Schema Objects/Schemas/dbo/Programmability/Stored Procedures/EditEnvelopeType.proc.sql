CREATE PROCEDURE [dbo].[EditEnvelopeType]
	@id int,
	@label nvarchar(50)
AS
BEGIN
 UPDATE [dbo].[EnvelopeTypes]
	SET Label = @label
	WHERE EnvelopeTypeId = @id;
END
