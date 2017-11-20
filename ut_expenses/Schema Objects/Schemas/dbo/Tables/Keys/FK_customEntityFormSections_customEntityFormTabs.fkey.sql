ALTER TABLE [dbo].[customEntityFormSections]
    ADD CONSTRAINT [FK_customEntityFormSections_customEntityFormTabs] FOREIGN KEY ([tabid]) REFERENCES [dbo].[customEntityFormTabs] ([tabid]) ON DELETE NO ACTION ON UPDATE SET NULL;

