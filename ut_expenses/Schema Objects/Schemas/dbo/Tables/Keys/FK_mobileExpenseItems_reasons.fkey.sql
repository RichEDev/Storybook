ALTER TABLE [dbo].[mobileExpenseItems] ADD CONSTRAINT [FK_mobileExpenseItems_reasons] FOREIGN KEY ([reasonid]) REFERENCES [dbo].[reasons] ([reasonid]) ON DELETE SET NULL
