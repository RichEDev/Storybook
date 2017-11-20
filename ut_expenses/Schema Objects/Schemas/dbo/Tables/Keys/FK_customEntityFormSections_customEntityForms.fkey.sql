ALTER TABLE [dbo].[customEntityFormSections]
    ADD CONSTRAINT [FK_customEntityFormSections_customEntityForms] FOREIGN KEY ([formid]) REFERENCES [dbo].[customEntityForms] ([formid]) ON DELETE CASCADE ON UPDATE NO ACTION;

