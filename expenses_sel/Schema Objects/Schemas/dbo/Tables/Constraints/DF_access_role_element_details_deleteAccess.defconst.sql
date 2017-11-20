ALTER TABLE [dbo].[accessRoleElementDetails]
    ADD CONSTRAINT [DF_access_role_element_details_deleteAccess] DEFAULT ((0)) FOR [deleteAccess];

