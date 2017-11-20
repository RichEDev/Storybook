ALTER TABLE [dbo].[customEntityFormTabs]
    ADD CONSTRAINT [FK_customEntityFormTabs_customEntityForms] FOREIGN KEY ([formid]) REFERENCES [dbo].[customEntityForms] ([formid]) ON DELETE CASCADE ON UPDATE NO ACTION;

