CREATE PROCEDURE [dbo].[SaveProcessedReceiptData](
	@WalletReceiptId INT
	,@Total DECIMAL(18,2)
	,@Date DATETIME
	,@Merchant NVARCHAR(100)
)
AS
DECLARE @returnValue INT;
BEGIN
	INSERT INTO [dbo].[ProcessedReceipts] (WalletReceiptId, [Date], Total, Merchant)
	VALUES (@WalletReceiptId, @Date, @Total, @Merchant)

	UPDATE [dbo].[WalletReceipts]
	SET [Status] = 1
	WHERE WalletReceiptId = @WalletReceiptId 

	RETURN SCOPE_IDENTITY();
END