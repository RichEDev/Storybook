ALTER TABLE [dbo].[elementsBase]
    ADD CONSTRAINT [DF_elements_base_accessRolesCanDelete] DEFAULT ((1)) FOR [accessRolesCanDelete];

