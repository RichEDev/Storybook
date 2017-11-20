ALTER TABLE [dbo].[attachment_audience]
    ADD CONSTRAINT [FK_attachment_audience_attachments] FOREIGN KEY ([attachmentId]) REFERENCES [dbo].[attachments] ([attachmentId]) ON DELETE CASCADE ON UPDATE NO ACTION;

