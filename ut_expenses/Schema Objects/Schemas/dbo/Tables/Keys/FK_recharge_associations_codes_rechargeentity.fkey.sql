ALTER TABLE [dbo].[recharge_associations]
    ADD CONSTRAINT [FK_recharge_associations_codes_rechargeentity] FOREIGN KEY ([rechargeEntityId]) REFERENCES [dbo].[codes_rechargeentity] ([entityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

