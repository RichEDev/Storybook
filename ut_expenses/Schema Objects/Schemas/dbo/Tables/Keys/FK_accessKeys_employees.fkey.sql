ALTER TABLE [dbo].[accessKeys]
    ADD CONSTRAINT [FK_accessKeys_employees] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

