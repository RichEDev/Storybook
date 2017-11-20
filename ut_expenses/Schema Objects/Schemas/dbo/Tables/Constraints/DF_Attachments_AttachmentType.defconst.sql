ALTER TABLE [dbo].[attachments]
    ADD CONSTRAINT [DF_Attachments_AttachmentType] DEFAULT ((0)) FOR [attachmentType];

