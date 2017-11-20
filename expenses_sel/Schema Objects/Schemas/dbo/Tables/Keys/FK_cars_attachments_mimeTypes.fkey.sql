ALTER TABLE [dbo].[cars_attachments]
    ADD CONSTRAINT [FK_cars_attachments_mimeTypes] FOREIGN KEY ([mimeID]) REFERENCES [dbo].[mimeTypes] ([mimeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

