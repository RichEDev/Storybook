CREATE TABLE [dbo].[mime_headers] (
    [mimeId]        INT            IDENTITY (1, 1) NOT NULL,
    [fileExtension] VARCHAR (10)   NULL,
    [mimeHeader]    NVARCHAR (150) NULL,
    [createdOn]     DATETIME       NULL,
    [createdBy]     INT            NULL,
    [modifiedOn]    DATETIME       NULL,
    [modifiedBy]    INT            NULL
);

