ALTER TABLE [dbo].[pending_email_attachments]
    ADD CONSTRAINT [PK_pending_email_attachments] PRIMARY KEY CLUSTERED ([pendingAttachmentId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

