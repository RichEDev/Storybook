ALTER TABLE [dbo].[accessRoleCustomEntityFormDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityFormDetails_custom_entity_forms] FOREIGN KEY ([customEntityFormID]) REFERENCES [dbo].[custom_entity_forms] ([formid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

