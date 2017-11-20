ALTER TABLE [dbo].[accessRoleCustomEntityFormDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityFormDetails_customEntityForms] FOREIGN KEY ([customEntityFormID]) REFERENCES [dbo].[customEntityForms] ([formid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

