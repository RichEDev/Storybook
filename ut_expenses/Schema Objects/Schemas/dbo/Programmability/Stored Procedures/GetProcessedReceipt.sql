CREATE PROCEDURE [dbo].[GetProcessedReceipt] (@WalletReceiptId INT)
AS
BEGIN
	DECLARE @StatusValue INT = (
			SELECT [Status]
			FROM WalletReceipts
			WHERE WalletReceiptId = @WalletReceiptId
			)

	IF (@StatusValue = 0)
	BEGIN
		UPDATE WalletReceipts
		SET [Status] = 1
		WHERE WalletReceiptId = @WalletReceiptId

		SELECT WalletReceiptId, FileExtension
			,STATUS
		FROM WalletReceipts
		WHERE WalletReceiptId = @WalletReceiptId
	END
	ELSE
	BEGIN

		SELECT -1 AS WalletReceiptId
	
	END
END