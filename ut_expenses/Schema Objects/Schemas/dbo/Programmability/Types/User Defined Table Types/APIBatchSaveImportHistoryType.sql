CREATE TYPE [dbo].[APIBatchSaveImportHistoryType] AS TABLE (
    [historyId]       INT      NULL,
    [importId]        INT      NULL,
    [logId]           INT      NULL,
    [importedDate]    DATETIME NULL,
    [importStatus]    INT      NULL,
    [applicationType] INT      NULL,
    [dataId]          INT      NULL);

