ALTER TABLE [dbo].[WalletReceipts]
	ADD CONSTRAINT [FK_WalletReceipts_employees]
	FOREIGN KEY ([CreatedBy])
	REFERENCES [dbo].[employees] ([employeeid])
	ON DELETE CASCADE