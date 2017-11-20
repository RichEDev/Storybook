ALTER TABLE [dbo].[range_exchangerates]
    ADD CONSTRAINT [FK_range_exchangerates_currencies] FOREIGN KEY ([tocurrencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE CASCADE ON UPDATE NO ACTION;

