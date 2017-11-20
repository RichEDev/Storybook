ALTER TABLE [dbo].[elementsBase]
    ADD CONSTRAINT [DF_elements_base_accessRolesCanEdit] DEFAULT ((1)) FOR [accessRolesCanEdit];

