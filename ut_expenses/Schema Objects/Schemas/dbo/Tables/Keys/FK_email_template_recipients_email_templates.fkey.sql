ALTER TABLE [dbo].[emailTemplateRecipients]
    ADD CONSTRAINT [FK_email_template_recipients_email_templates] FOREIGN KEY ([emailtemplateid]) REFERENCES [dbo].[emailTemplates] ([emailtemplateid]) ON DELETE CASCADE ON UPDATE NO ACTION;

