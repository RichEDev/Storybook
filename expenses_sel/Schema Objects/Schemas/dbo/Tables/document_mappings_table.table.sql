CREATE TABLE [dbo].[document_mappings_table] (
    [mappingid]         INT              NOT NULL,
    [oldreportcolumnid] INT              NULL,
    [columnidx]         TINYINT          NOT NULL,
    [width]             DECIMAL (18, 2)  NULL,
    [reportcolumnid]    UNIQUEIDENTIFIER NULL,
    [headertext]        NVARCHAR (100)   NULL,
    [table_mappingid]   UNIQUEIDENTIFIER NOT NULL,
    [columntextbold]    BIT              NOT NULL,
    [createdOn]         DATETIME         NULL,
    [createdBy]         INT              NULL,
    [modifiedOn]        DATETIME         NULL,
    [modifiedBy]        INT              NULL,
    [hiddencolumnid]    UNIQUEIDENTIFIER NOT NULL
);

