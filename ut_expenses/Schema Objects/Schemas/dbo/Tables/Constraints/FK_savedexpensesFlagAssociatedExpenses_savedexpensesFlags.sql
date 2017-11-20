ALTER TABLE [dbo].[savedexpensesFlagAssociatedExpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpensesFlagAssociatedExpenses_savedexpensesFlags] FOREIGN KEY([flaggedItemId])
REFERENCES [dbo].[savedexpensesFlags] ([flaggedItemId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[savedexpensesFlagAssociatedExpenses] CHECK CONSTRAINT [FK_savedexpensesFlagAssociatedExpenses_savedexpensesFlags]
GO
