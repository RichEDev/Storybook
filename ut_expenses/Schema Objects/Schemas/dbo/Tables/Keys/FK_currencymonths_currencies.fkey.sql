ALTER TABLE [dbo].[currencymonths]
    ADD CONSTRAINT [FK_currencymonths_currencies] FOREIGN KEY ([currencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE CASCADE ON UPDATE CASCADE;

