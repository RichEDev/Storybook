ALTER TABLE [dbo].[accessRoleCustomEntityViewDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityViewDetails_custom_entity_views] FOREIGN KEY ([customEntityViewID]) REFERENCES [dbo].[custom_entity_views] ([viewid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

