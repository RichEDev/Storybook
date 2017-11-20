ALTER TABLE [dbo].[range_exchangerates]
    ADD CONSTRAINT [FK_range_exchangerates_currencyranges] FOREIGN KEY ([currencyrangeid]) REFERENCES [dbo].[currencyranges] ([currencyrangeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

