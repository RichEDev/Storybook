ALTER TABLE [dbo].[accessRoles]
    ADD CONSTRAINT [DF_access_roles_employeesCanAmendDesignatedProjectCode] DEFAULT ((1)) FOR [employeesCanAmendDesignatedProjectCode];

