ALTER TABLE [dbo].[userdefined_employees]
    ADD CONSTRAINT [FK_userdefined_employees_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

