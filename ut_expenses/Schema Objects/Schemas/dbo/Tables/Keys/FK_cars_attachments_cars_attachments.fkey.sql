ALTER TABLE [dbo].[cars_attachments]
    ADD CONSTRAINT [FK_cars_attachments_cars_attachments] FOREIGN KEY ([attachmentID]) REFERENCES [dbo].[cars_attachments] ([attachmentID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

