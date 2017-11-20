ALTER TABLE [dbo].[employeePasswordKeys]
    ADD CONSTRAINT [FK_employeePasswordKeys_employees] FOREIGN KEY ([employeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

