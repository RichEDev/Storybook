CREATE PROCEDURE [dbo].[AddReceipt]
	@fileExtension nvarchar(6),
	@creationMethod tinyint,
	@expediteUsername nvarchar(50) = NULL,
	@envelopeId int = NULL,
	@userId int	
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Receipts (FileExtension, CreationMethod, Deleted, ExpediteUsername, EnvelopeId, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn) 
	VALUES (@fileExtension, @creationMethod, 0, @expediteUsername, @envelopeId, @userId, CURRENT_TIMESTAMP, @userId, CURRENT_TIMESTAMP); 

	DECLARE @receiptId INT = SCOPE_IDENTITY();

    DECLARE @log nvarchar(4000) = 'Receipt (' + CONVERT(NVARCHAR, @receiptId) + ') added';
	EXEC addInsertEntryToAuditLog @userId, @userId, 186, @receiptId, @log, NULL;

	RETURN @receiptId;
END