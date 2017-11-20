ALTER TABLE [dbo].[elementsBase]
    ADD CONSTRAINT [DF_elementsBase_accessRolesApplicable] DEFAULT ((1)) FOR [accessRolesApplicable];

