ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [FK_recharge_associations_contract_productdetails] FOREIGN KEY ([contractProductId]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

