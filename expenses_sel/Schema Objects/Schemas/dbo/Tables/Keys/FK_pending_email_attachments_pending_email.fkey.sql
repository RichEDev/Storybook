ALTER TABLE [dbo].[pending_email_attachments]
    ADD CONSTRAINT [FK_pending_email_attachments_pending_email] FOREIGN KEY ([pendingEmailId]) REFERENCES [dbo].[pending_email] ([emailId]) ON DELETE CASCADE ON UPDATE NO ACTION;

