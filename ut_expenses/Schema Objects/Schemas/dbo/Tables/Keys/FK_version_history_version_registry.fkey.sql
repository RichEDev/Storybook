ALTER TABLE [dbo].[version_history]
    ADD CONSTRAINT [FK_version_history_version_registry] FOREIGN KEY ([RegistryId]) REFERENCES [dbo].[version_registry] ([RegistryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

