ALTER TABLE [dbo].[contract_productdetails_oneoffcharge]
    ADD CONSTRAINT [FK_contract_productdetails_oneoffcharge_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

