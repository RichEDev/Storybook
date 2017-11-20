CREATE TABLE [dbo].[document_mappings_mergetable] (
    [mergetable_mappingid] UNIQUEIDENTIFIER NOT NULL,
    [mappingid]            INT              NOT NULL,
    [table_mappingid]      INT              NOT NULL,
    [sequence]             TINYINT          NULL,
    [createdOn]            DATETIME         NULL,
    [createdBy]            INT              NULL,
    [modifiedOn]           DATETIME         NULL,
    [modifiedBy]           INT              NULL
);

