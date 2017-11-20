ALTER TABLE [dbo].[link_variations]
    ADD CONSTRAINT [FK_link_variations_contract_details1] FOREIGN KEY ([variationContractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

