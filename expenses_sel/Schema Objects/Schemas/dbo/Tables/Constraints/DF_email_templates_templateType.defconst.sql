ALTER TABLE [dbo].[email_templates]
    ADD CONSTRAINT [DF_email_templates_templateType] DEFAULT ((0)) FOR [templateType];

