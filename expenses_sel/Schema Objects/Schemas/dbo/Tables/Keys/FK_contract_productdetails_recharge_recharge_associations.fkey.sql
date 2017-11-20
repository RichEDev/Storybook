ALTER TABLE [dbo].[contract_productdetails_recharge]
    ADD CONSTRAINT [FK_contract_productdetails_recharge_recharge_associations] FOREIGN KEY ([rechargeId]) REFERENCES [dbo].[recharge_associations] ([rechargeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

