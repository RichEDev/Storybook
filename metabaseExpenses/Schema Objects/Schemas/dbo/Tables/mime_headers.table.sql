CREATE TABLE [dbo].[mime_headers] (
    [old_mimeID]    INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [fileExtension] NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [mimeHeader]    NVARCHAR (150)   COLLATE Latin1_General_CI_AS NOT NULL,
    [mimeID]        UNIQUEIDENTIFIER CONSTRAINT [DF_mime_headers_mimeID] DEFAULT (newid()) NOT NULL,
    [description]   NVARCHAR (500)   COLLATE Latin1_General_CI_AS NULL,
    CONSTRAINT [PK_mime_headers] PRIMARY KEY CLUSTERED ([mimeID] ASC)
);



