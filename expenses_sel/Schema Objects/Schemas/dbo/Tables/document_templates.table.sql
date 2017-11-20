﻿CREATE TABLE [dbo].[document_templates] (
    [documentid]      INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [doc_path]        NVARCHAR (250) NOT NULL,
    [doc_filename]    NVARCHAR (150) NOT NULL,
    [doc_description] NVARCHAR (MAX) NULL,
    [createddate]     DATETIME       NULL,
    [createdby]       INT            NULL,
    [modifieddate]    DATETIME       NULL,
    [modifiedby]      INT            NULL,
    [doc_contenttype] NVARCHAR (100) NOT NULL,
    [doc_name]        NVARCHAR (150) NOT NULL,
    [mergeprojectid]  INT            NOT NULL
);

