ALTER TABLE [dbo].[monthly_exchangerates]
    ADD CONSTRAINT [FK_monthly_exchangerates_currencies] FOREIGN KEY ([tocurrencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

