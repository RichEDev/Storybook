ALTER TABLE [dbo].[emailTemplateBodyFields]
    ADD CONSTRAINT [FK_emailTemplateBodyFields_emailTemplates] FOREIGN KEY ([emailtemplateid]) REFERENCES [dbo].[emailTemplates] ([emailtemplateid]) ON DELETE CASCADE ON UPDATE NO ACTION;

