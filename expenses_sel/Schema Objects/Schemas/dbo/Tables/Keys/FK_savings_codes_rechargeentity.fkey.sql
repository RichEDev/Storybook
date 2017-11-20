ALTER TABLE [dbo].[savings]
    ADD CONSTRAINT [FK_savings_codes_rechargeentity] FOREIGN KEY ([rechargeEntityId]) REFERENCES [dbo].[codes_rechargeentity] ([entityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

