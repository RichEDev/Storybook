ALTER TABLE [dbo].[employee_roles]
    ADD CONSTRAINT [FK_employee_roles_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

