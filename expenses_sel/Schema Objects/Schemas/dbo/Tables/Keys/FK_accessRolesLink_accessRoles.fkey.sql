ALTER TABLE [dbo].[accessRolesLink]
    ADD CONSTRAINT [FK_accessRolesLink_accessRoles] FOREIGN KEY ([primaryAccessRoleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

