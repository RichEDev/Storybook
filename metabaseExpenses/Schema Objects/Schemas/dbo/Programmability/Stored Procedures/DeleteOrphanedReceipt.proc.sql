CREATE PROCEDURE [dbo].[DeleteOrphanedReceipt]
	@receiptId INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM OrphanedReceipts
	WHERE ReceiptId = @receiptId;
END