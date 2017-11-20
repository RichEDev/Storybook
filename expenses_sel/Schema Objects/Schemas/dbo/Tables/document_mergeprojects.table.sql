CREATE TABLE [dbo].[document_mergeprojects] (
    [mergeprojectid]      INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [project_name]        NVARCHAR (150) NOT NULL,
    [project_description] NVARCHAR (MAX) NULL,
    [createddate]         DATETIME       NOT NULL,
    [createdby]           INT            NOT NULL,
    [modifieddate]        DATETIME       NOT NULL,
    [modifiedby]          INT            NOT NULL,
    [complete]            BIT            NOT NULL
);

