ALTER TABLE [dbo].[accessRoleCustomEntityFormDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityFormDetails_custom_entities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

