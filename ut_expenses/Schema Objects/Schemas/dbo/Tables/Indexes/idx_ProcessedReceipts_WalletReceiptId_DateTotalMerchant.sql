CREATE NONCLUSTERED INDEX [idx_ProcessedReceipts_WalletReceiptId_DateTotalMerchant] ON [dbo].[ProcessedReceipts] ([WalletReceiptId] ASC) INCLUDE (
	[Date]
	,[Total]
	,[Merchant]
	)