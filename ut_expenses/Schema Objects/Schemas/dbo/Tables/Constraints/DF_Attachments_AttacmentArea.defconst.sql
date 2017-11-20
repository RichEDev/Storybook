ALTER TABLE [dbo].[attachments]
    ADD CONSTRAINT [DF_Attachments_AttacmentArea] DEFAULT ((0)) FOR [attachmentArea];

