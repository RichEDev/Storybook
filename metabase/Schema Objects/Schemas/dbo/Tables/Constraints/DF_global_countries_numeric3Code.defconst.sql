ALTER TABLE [dbo].[global_countries]
    ADD CONSTRAINT [DF_global_countries_numeric3Code] DEFAULT ((0)) FOR [numeric3Code];

