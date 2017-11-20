ALTER TABLE [dbo].[emailTemplateSubjectFields]
    ADD CONSTRAINT [FK_email_template_subject_fields_email_templates] FOREIGN KEY ([emailtemplateid]) REFERENCES [dbo].[emailTemplates] ([emailtemplateid]) ON DELETE CASCADE ON UPDATE NO ACTION;

