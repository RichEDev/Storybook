ALTER TABLE [dbo].[employee_attachments]
    ADD CONSTRAINT [FK_employee_attachments_mimeTypes] FOREIGN KEY ([mimeID]) REFERENCES [dbo].[mimeTypes] ([mimeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

