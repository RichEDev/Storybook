ALTER TABLE [dbo].[accessRoleCustomEntityFormDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityFormDetails_accessRoles] FOREIGN KEY ([roleID]) REFERENCES [dbo].[accessRoles] ([roleID]) ON DELETE CASCADE ON UPDATE NO ACTION;

