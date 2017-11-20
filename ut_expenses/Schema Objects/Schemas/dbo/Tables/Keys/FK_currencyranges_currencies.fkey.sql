ALTER TABLE [dbo].[currencyranges]
    ADD CONSTRAINT [FK_currencyranges_currencies] FOREIGN KEY ([currencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE CASCADE ON UPDATE CASCADE;

