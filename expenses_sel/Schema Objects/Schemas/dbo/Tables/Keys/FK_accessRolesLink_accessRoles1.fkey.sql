ALTER TABLE [dbo].[accessRolesLink]
    ADD CONSTRAINT [FK_accessRolesLink_accessRoles1] FOREIGN KEY ([secondaryAccessRoleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE NO ACTION;

