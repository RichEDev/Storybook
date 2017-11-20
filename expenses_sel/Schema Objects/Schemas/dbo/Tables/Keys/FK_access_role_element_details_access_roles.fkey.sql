ALTER TABLE [dbo].[accessRoleElementDetails]
    ADD CONSTRAINT [FK_access_role_element_details_access_roles] FOREIGN KEY ([roleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE CASCADE;

