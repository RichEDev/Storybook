-- Foreign Keys

ALTER TABLE [dbo].[flags] ADD CONSTRAINT [FK_flags_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE SET NULL

