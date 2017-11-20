ALTER TABLE [dbo].[accessRoleElementDetails]
    ADD CONSTRAINT [DF_access_role_element_details_updateAccess] DEFAULT ((0)) FOR [updateAccess];

