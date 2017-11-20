ALTER TABLE [dbo].[accessRoleCustomEntityDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityDetails_custom_entities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

