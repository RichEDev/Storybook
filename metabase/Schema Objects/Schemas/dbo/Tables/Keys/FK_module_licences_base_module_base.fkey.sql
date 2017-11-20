ALTER TABLE [dbo].[moduleLicencesBase]
    ADD CONSTRAINT [FK_module_licences_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) ON DELETE CASCADE ON UPDATE CASCADE NOT FOR REPLICATION;

