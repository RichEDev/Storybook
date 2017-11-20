ALTER TABLE [dbo].[bankAccountValidation]
    ADD CONSTRAINT [FK_bankAccountValidation_employees] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

