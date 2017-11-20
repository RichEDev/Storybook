CREATE TABLE [dbo].[pending_email_attachments] (
    [pendingAttachmentId] INT IDENTITY (1, 1) NOT NULL,
    [pendingEmailId]      INT NOT NULL,
    [attachmentId]        INT NOT NULL
);

