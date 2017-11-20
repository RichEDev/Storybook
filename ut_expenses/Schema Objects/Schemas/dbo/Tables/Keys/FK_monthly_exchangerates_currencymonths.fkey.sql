ALTER TABLE [dbo].[monthly_exchangerates]
    ADD CONSTRAINT [FK_monthly_exchangerates_currencymonths] FOREIGN KEY ([currencymonthid]) REFERENCES [dbo].[currencymonths] ([currencymonthid]) ON DELETE CASCADE ON UPDATE CASCADE;

