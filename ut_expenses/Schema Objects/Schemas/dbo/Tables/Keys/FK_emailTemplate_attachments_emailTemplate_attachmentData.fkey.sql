ALTER TABLE [dbo].[emailTemplate_attachments]
    ADD CONSTRAINT [FK_emailTemplate_attachments_emailTemplate_attachmentData] FOREIGN KEY ([attachmentID]) REFERENCES [dbo].[emailTemplate_attachmentData] ([attachmentID]) ON DELETE CASCADE ON UPDATE NO ACTION;

