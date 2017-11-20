ALTER TABLE [dbo].[static_exchangerates]
    ADD CONSTRAINT [FK_static_exchangerates_currencies] FOREIGN KEY ([currencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE CASCADE ON UPDATE CASCADE;

