CREATE TYPE [dbo].[ApiBatchSaveEsrElementFieldsType] AS TABLE (
    [elementFieldID]       INT              NULL,
    [elementID]            INT              NULL,
    [globalElementFieldID] INT              NULL,
    [aggregate]            TINYINT          NULL,
    [order]                TINYINT          NULL,
    [reportColumnID]       UNIQUEIDENTIFIER NULL);

