ALTER TABLE [dbo].[mobileExpenseItems] ADD CONSTRAINT [FK_mobileExpenseItems_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE
