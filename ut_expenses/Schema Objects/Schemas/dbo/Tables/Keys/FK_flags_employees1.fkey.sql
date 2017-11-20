ALTER TABLE [dbo].[flags] ADD CONSTRAINT [FK_flags_employees1] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[employees] ([employeeid])

