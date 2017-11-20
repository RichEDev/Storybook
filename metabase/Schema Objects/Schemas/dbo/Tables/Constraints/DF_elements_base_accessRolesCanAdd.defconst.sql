ALTER TABLE [dbo].[elementsBase]
    ADD CONSTRAINT [DF_elements_base_accessRolesCanAdd] DEFAULT ((1)) FOR [accessRolesCanAdd];

