ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [FK_customEntities_currencies] FOREIGN KEY ([defaultCurrencyID]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

