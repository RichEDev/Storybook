ALTER TABLE [dbo].[contract_producthistory]
    ADD CONSTRAINT [FK_contract_producthistory_contract_productdetails] FOREIGN KEY ([contractProductId]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE CASCADE ON UPDATE NO ACTION;

