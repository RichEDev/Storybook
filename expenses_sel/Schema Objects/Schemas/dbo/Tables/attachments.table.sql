CREATE TABLE [dbo].[attachments] (
    [attachmentId]    INT            IDENTITY (1, 1) NOT NULL,
    [referenceNumber] INT            NULL,
    [Directory]       NVARCHAR (250) NULL,
    [Filename]        NVARCHAR (100) NULL,
    [Description]     VARCHAR (100)  NULL,
    [dateAttached]    DATETIME       NULL,
    [attachmentType]  INT            NULL,
    [attachmentArea]  INT            NOT NULL,
    [mimeHeader]      NVARCHAR (150) NULL,
    [attachedBy]      VARCHAR (60)   NULL
);

