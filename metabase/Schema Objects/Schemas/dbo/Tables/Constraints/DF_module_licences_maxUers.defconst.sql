ALTER TABLE [dbo].[moduleLicencesBase]
    ADD CONSTRAINT [DF_module_licences_maxUers] DEFAULT ((0)) FOR [maxUsers];

