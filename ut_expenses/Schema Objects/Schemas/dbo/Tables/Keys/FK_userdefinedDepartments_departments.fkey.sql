ALTER TABLE [dbo].[userdefinedDepartments]
    ADD CONSTRAINT [FK_userdefinedDepartments_departments] FOREIGN KEY ([departmentid]) REFERENCES [dbo].[departments] ([departmentid]) ON DELETE CASCADE ON UPDATE NO ACTION;

