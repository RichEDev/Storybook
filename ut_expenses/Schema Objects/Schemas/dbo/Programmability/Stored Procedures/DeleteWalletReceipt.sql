CREATE PROCEDURE [dbo].[DeleteWalletReceipt](
	@walletReceiptId INT
	,@CUemployeeID INT
	,@CUdelegateID INT
)
AS
DECLARE @returnValue INT;
BEGIN
	IF EXISTS(SELECT WalletReceiptId FROM WalletReceipts WHERE WalletReceiptId = @walletReceiptId and CreatedBy = @CUemployeeID)
	BEGIN
		DELETE WalletReceipts WHERE WalletReceiptId = @walletReceiptId
		DELETE ProcessedReceipts WHERE WalletReceiptId = @walletReceiptId

		EXEC addDeleteEntryToAuditLog @CUemployeeID
			,@CUdelegateID
			,199
			,@walletReceiptId
			,NULL
			,NULL;

		RETURN 0;
	END
	ELSE
	BEGIN
		RETURN -1;
	END
END