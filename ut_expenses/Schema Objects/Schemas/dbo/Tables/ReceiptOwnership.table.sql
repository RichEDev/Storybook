CREATE TABLE [dbo].[ReceiptOwnership]
(	
	[ReceiptId] [int] NOT NULL,
	[SavedExpenseId] [int] NOT NULL	,
	PRIMARY KEY (ReceiptId, SavedExpenseId),
	CONSTRAINT [FK_ReceiptOwnership_Receipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[Receipts] ([ReceiptId]) ON DELETE CASCADE,
	CONSTRAINT [FK_ReceiptOwnership_SavedExpenses] FOREIGN KEY([SavedExpenseId]) REFERENCES [dbo].[savedexpenses] ([expenseid]) ON DELETE CASCADE
)
GO
CREATE INDEX Index_ReceiptOwnership_Receipt ON [dbo].[ReceiptOwnership] (ReceiptId)
GO
CREATE INDEX Index_ReceiptOwnership_Saved ON [dbo].[ReceiptOwnership] (SavedExpenseId)
GO