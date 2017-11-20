ALTER TABLE [dbo].[contract_forecastdetails]
    ADD CONSTRAINT [FK_contract_forecastdetails_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

