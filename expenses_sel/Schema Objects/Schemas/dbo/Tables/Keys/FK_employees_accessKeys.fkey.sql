ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [FK_employees_accessKeys] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

