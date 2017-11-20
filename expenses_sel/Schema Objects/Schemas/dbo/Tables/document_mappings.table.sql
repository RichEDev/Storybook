CREATE TABLE [dbo].[document_mappings] (
    [mappingid]       INT            IDENTITY (1, 1) NOT NULL,
    [mergeprojectid]  INT            NULL,
    [mergesourceid]   INT            NULL,
    [merge_fieldtype] INT            NOT NULL,
    [merge_fieldkey]  NVARCHAR (200) NULL,
    [includeHeaders]  BIT            NULL,
    [headerFont]      NVARCHAR (50)  NULL,
    [headerFontSize]  DECIMAL (18)   NULL,
    [bodyFont]        NVARCHAR (50)  NULL,
    [bodyFontSize]    DECIMAL (18)   NULL,
    [isMergePart]     BIT            NOT NULL,
    [createdOn]       DATETIME       NULL,
    [createdBy]       INT            NULL,
    [modifiedOn]      DATETIME       NULL,
    [modifiedBy]      INT            NULL,
    [noDataMessage]   NVARCHAR (MAX) NULL,
    [description]     NVARCHAR (100) NULL
);

