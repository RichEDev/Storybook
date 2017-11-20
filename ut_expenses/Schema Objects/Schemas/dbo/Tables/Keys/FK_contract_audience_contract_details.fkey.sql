ALTER TABLE [dbo].[contract_audience]
    ADD CONSTRAINT [FK_contract_audience_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

