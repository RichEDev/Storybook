ALTER TABLE [dbo].[accessRoleCustomEntityDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityDetails_customEntities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

