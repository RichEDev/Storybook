CREATE TABLE [dbo].[cars_attachments] (
    [attachmentID] INT            NOT NULL,
    [id]           INT            NOT NULL,
    [title]        NVARCHAR (200) NOT NULL,
    [description]  NVARCHAR (MAX) NULL,
    [fileName]     NVARCHAR (150) NULL,
    [mimeID]       INT            NOT NULL,
    [createdon]    DATETIME       NULL,
    [createdby]    INT            NOT NULL
);

