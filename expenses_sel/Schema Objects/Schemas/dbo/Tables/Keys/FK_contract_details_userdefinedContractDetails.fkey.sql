ALTER TABLE [dbo].[userdefinedContractDetails]
    ADD CONSTRAINT [FK_contract_details_userdefinedContractDetails] FOREIGN KEY ([contractid]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

