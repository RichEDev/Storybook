ALTER TABLE [dbo].[contract_history]
    ADD CONSTRAINT [FK_contract_history_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

