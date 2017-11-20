ALTER TABLE [dbo].[recharge_servicedates]
    ADD CONSTRAINT [FK_recharge_servicedates_recharge_associations] FOREIGN KEY ([rechargeId]) REFERENCES [dbo].[recharge_associations] ([rechargeId]) ON DELETE CASCADE ON UPDATE NO ACTION;

