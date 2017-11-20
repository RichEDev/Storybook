-- Foreign Keys

ALTER TABLE [dbo].[savedexpensesFlags] ADD CONSTRAINT [FK_savedexpensesFlags_flags] FOREIGN KEY ([flagID]) REFERENCES [dbo].[flags] ([flagID])

