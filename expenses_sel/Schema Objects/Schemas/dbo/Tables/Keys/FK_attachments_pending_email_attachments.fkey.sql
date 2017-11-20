ALTER TABLE [dbo].[pending_email_attachments]
    ADD CONSTRAINT [FK_attachments_pending_email_attachments] FOREIGN KEY ([attachmentId]) REFERENCES [dbo].[attachments] ([attachmentId]) ON DELETE CASCADE ON UPDATE NO ACTION;

