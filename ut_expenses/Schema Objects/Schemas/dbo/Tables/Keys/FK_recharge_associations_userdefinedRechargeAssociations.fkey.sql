ALTER TABLE [dbo].[userdefinedRechargeAssociations]
    ADD CONSTRAINT [FK_recharge_associations_userdefinedRechargeAssociations] FOREIGN KEY ([rechargeid]) REFERENCES [dbo].[recharge_associations] ([rechargeId]) ON DELETE CASCADE ON UPDATE NO ACTION;

