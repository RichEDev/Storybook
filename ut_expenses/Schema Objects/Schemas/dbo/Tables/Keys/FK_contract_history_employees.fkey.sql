ALTER TABLE [dbo].[contract_history]
    ADD CONSTRAINT [FK_contract_history_employees] FOREIGN KEY ([employeeId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

