ALTER TABLE [dbo].[customEntityFormFields]
    ADD CONSTRAINT [FK_customEntityFormFields_customEntityFormSections] FOREIGN KEY ([sectionid]) REFERENCES [dbo].[customEntityFormSections] ([sectionid]) ON DELETE NO ACTION ON UPDATE SET NULL;

