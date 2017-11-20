ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_employees] FOREIGN KEY ([contractOwner]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

