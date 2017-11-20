ALTER TABLE [dbo].[accessRoleCustomEntityViewDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityViewDetails_customEntities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

