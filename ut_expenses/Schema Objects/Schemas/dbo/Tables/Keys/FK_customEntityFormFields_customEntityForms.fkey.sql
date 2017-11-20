ALTER TABLE [dbo].[customEntityFormFields]
    ADD CONSTRAINT [FK_customEntityFormFields_customEntityForms] FOREIGN KEY ([formid]) REFERENCES [dbo].[customEntityForms] ([formid]) ON DELETE CASCADE ON UPDATE NO ACTION;

