ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_ForecastTypeId] DEFAULT ((0)) FOR [forecastTypeId];

