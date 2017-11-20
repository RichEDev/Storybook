ALTER TABLE [dbo].[employeeAccessRoles]
    ADD CONSTRAINT [FK_employee_access_roles_access_roles] FOREIGN KEY ([accessRoleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE CASCADE;

