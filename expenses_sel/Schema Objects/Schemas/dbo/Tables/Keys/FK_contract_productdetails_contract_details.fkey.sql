ALTER TABLE [dbo].[contract_productdetails]
    ADD CONSTRAINT [FK_contract_productdetails_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

