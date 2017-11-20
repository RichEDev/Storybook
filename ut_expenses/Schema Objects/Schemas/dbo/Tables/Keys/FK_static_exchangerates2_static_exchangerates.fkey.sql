ALTER TABLE [dbo].[static_exchangerates]
    ADD CONSTRAINT [FK_static_exchangerates2_static_exchangerates] FOREIGN KEY ([tocurrencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

