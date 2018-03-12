ALTER TABLE [dbo].[ProcessedReceipts]
	ADD CONSTRAINT [FK_ProcessedReceipts_WalletReceipts]
	FOREIGN KEY ([WalletReceiptId])
	REFERENCES [dbo].[WalletReceipts] ([WalletReceiptId])
	ON DELETE CASCADE