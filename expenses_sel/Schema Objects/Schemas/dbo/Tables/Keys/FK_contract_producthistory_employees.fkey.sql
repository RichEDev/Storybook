ALTER TABLE [dbo].[contract_producthistory]
    ADD CONSTRAINT [FK_contract_producthistory_employees] FOREIGN KEY ([employeeId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

