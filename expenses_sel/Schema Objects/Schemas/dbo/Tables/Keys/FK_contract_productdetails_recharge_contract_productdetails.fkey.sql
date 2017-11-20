ALTER TABLE [dbo].[contract_productdetails_recharge]
    ADD CONSTRAINT [FK_contract_productdetails_recharge_contract_productdetails] FOREIGN KEY ([contractProductId]) REFERENCES [dbo].[contract_productdetails] ([contractProductId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

