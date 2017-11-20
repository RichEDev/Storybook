ALTER TABLE [dbo].[userdefinedContractForecastDetails]
    ADD CONSTRAINT [FK_invoice_forecasts_userdefinedContractForecastDetails] FOREIGN KEY ([forecastid]) REFERENCES [dbo].[contract_forecastdetails] ([contractForecastId]) ON DELETE CASCADE ON UPDATE NO ACTION;

