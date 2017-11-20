ALTER TABLE [dbo].[contract_notification]
    ADD CONSTRAINT [FK_contract_notification_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

