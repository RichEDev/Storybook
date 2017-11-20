ALTER TABLE [dbo].[importTemplates]
    ADD CONSTRAINT [DF_importTemplates_isAutomated] DEFAULT ((0)) FOR [isAutomated];

