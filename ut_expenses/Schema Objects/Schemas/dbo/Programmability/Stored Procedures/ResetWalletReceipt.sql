CREATE PROCEDURE [dbo].[ResetWalletReceipt](
	@WalletReceiptId INT
)
AS
BEGIN

	UPDATE WalletReceipts
	SET [Status] = 0
	WHERE WalletReceiptId = @WalletReceiptId

END