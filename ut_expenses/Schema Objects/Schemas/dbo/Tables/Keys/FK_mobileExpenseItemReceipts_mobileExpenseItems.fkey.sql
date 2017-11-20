ALTER TABLE [dbo].[mobileExpenseItemReceipts]
	ADD CONSTRAINT [FK_mobileExpenseItemReceipts_mobileExpenseItems] FOREIGN KEY ([mobileID]) REFERENCES [dbo].[mobileExpenseItems] ([mobileID]) ON DELETE CASCADE

