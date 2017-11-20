ALTER TABLE [dbo].[contract_forecastdetails]
    ADD CONSTRAINT [DF_Contract_ForecastDetails_POMaxValue] DEFAULT ((0)) FOR [poMaxValue];

