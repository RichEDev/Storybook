ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [FK_custom_entities_currencies] FOREIGN KEY ([defaultCurrencyID]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

