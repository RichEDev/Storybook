ALTER TABLE [dbo].[savings]
    ADD CONSTRAINT [FK_savings_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

