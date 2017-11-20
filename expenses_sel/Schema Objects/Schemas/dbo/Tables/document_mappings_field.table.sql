CREATE TABLE [dbo].[document_mappings_field] (
    [mappingid]         INT              NOT NULL,
    [oldreportcolumnid] INT              NULL,
    [reportcolumnid]    UNIQUEIDENTIFIER NULL,
    [createdOn]         DATETIME         NULL,
    [createdBy]         INT              NULL,
    [modifiedOn]        DATETIME         NULL,
    [modifiedBy]        INT              NULL
);

