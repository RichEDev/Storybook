ALTER TABLE [dbo].[accessRoles]
    ADD CONSTRAINT [DF_access_roles_employeesCanAmendDesignatedDepartment] DEFAULT ((1)) FOR [employeesCanAmendDesignatedDepartment];

