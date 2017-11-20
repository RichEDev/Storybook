ALTER TABLE [dbo].[contractNotes]
    ADD CONSTRAINT [FK_contract_notes_contract_details] FOREIGN KEY ([contractID]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

