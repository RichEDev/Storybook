CREATE TABLE [dbo].[emailTemplate_attachments] (
    [attachmentID] INT             NOT NULL,
    [id]           INT             NOT NULL,
    [title]        NVARCHAR (200)  NOT NULL,
    [description]  NVARCHAR (4000) NULL,
    [fileName]     NVARCHAR (150)  NULL,
    [mimeID]       INT             NOT NULL,
    [createdon]    DATETIME        NULL,
    [createdby]    INT             NOT NULL
);

