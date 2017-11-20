CREATE TABLE [dbo].[document_merge_association] (
    [docmergeassociationid] INT      IDENTITY (1, 1) NOT NULL,
    [documentid]            INT      NOT NULL,
    [entityid]              INT      NOT NULL,
    [recordid]              INT      NOT NULL,
    [createddate]           DATETIME NOT NULL,
    [createdby]             INT      NOT NULL,
    [modifieddate]          DATETIME NULL,
    [modifiedby]            INT      NULL
);

