ALTER TABLE [dbo].[elementCategoryBase]
    ADD CONSTRAINT [FK_elementCategoryBase_moduleBase] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) ON DELETE NO ACTION ON UPDATE NO ACTION NOT FOR REPLICATION;

