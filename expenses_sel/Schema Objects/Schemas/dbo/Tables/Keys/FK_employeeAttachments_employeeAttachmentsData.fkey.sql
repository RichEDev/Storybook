ALTER TABLE [dbo].[employee_attachments]
    ADD CONSTRAINT [FK_employeeAttachments_employeeAttachmentsData] FOREIGN KEY ([attachmentID]) REFERENCES [dbo].[employee_attachmentData] ([attachmentID]) ON DELETE CASCADE ON UPDATE NO ACTION;

