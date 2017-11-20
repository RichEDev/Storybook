ALTER TABLE [dbo].[accessRoleCustomEntityViewDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityViewDetails_accessRoles] FOREIGN KEY ([roleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE NO ACTION;

