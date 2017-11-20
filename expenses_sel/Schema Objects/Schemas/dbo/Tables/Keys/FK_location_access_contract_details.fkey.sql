ALTER TABLE [dbo].[subAccountAccess]
    ADD CONSTRAINT [FK_location_access_contract_details] FOREIGN KEY ([lastContractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

