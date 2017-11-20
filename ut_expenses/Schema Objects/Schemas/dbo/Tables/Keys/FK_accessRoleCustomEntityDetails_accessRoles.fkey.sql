ALTER TABLE [dbo].[accessRoleCustomEntityDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityDetails_accessRoles] FOREIGN KEY ([roleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE NO ACTION;

