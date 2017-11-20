ALTER TABLE [dbo].[accessRoleCustomEntityViewDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityViewDetails_custom_entities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

