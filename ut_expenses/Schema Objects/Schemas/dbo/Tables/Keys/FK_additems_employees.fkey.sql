ALTER TABLE [dbo].[additems]
    ADD CONSTRAINT [FK_additems_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

