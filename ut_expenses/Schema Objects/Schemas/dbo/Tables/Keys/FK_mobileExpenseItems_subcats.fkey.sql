ALTER TABLE [dbo].[mobileExpenseItems] ADD CONSTRAINT [FK_mobileExpenseItems_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE SET NULL
