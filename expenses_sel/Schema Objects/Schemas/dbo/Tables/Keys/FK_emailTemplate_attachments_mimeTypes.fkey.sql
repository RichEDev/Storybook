ALTER TABLE [dbo].[emailTemplate_attachments]
    ADD CONSTRAINT [FK_emailTemplate_attachments_mimeTypes] FOREIGN KEY ([mimeID]) REFERENCES [dbo].[mimeTypes] ([mimeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

