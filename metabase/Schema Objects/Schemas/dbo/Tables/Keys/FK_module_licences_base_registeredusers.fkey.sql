ALTER TABLE [dbo].[moduleLicencesBase]
    ADD CONSTRAINT [FK_module_licences_base_registeredusers] FOREIGN KEY ([accountID]) REFERENCES [dbo].[registeredusers] ([accountid]) ON DELETE CASCADE ON UPDATE CASCADE;

