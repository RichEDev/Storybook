ALTER TABLE [dbo].[contract_forecastproducts]
    ADD CONSTRAINT [FK_contract_forecastproducts_contract_forecastdetails] FOREIGN KEY ([forecastId]) REFERENCES [dbo].[contract_forecastdetails] ([contractForecastId]) ON DELETE CASCADE ON UPDATE NO ACTION;

