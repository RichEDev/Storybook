ALTER TABLE [dbo].[accessRoleCustomEntityFormDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityFormDetails_customEntities] FOREIGN KEY ([customEntityID]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

