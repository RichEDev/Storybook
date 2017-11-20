ALTER TABLE [dbo].[accessRoles]
    ADD CONSTRAINT [DF_access_roles_employeesCanAmendDesignatedCostCode] DEFAULT ((1)) FOR [employeesCanAmendDesignatedCostCode];

