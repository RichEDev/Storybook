ALTER TABLE [dbo].[contract_productdetails_recharge]
    ADD CONSTRAINT [FK_contract_productdetails_recharge_codes_rechargeentity] FOREIGN KEY ([rechargeEntityId]) REFERENCES [dbo].[codes_rechargeentity] ([entityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

