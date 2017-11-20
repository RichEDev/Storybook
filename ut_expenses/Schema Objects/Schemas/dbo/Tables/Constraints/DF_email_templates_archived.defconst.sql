ALTER TABLE [dbo].[emailTemplates]
    ADD CONSTRAINT [DF_email_templates_archived] DEFAULT ((0)) FOR [archived];

