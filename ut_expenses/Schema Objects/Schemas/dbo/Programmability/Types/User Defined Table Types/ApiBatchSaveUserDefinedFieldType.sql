CREATE TYPE [dbo].[ApiBatchSaveUserDefinedFieldType] AS TABLE (
    [userdefineid] INT              NULL,
    [field]        NVARCHAR (500)   NULL,
    [fieldid]      UNIQUEIDENTIFIER NULL,
    [tablename]    NVARCHAR (500)   NULL,
    [tableid]      UNIQUEIDENTIFIER NULL,
    [fieldtype]    TINYINT          NULL,
    [value]        NVARCHAR (4000)  NULL,
    [recordId]     INT              NULL,
    [displayField] UNIQUEIDENTIFIER NULL,
    [relatedtable] UNIQUEIDENTIFIER NULL);

