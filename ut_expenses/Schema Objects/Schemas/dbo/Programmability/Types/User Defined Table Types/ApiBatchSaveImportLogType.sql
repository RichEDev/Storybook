CREATE TYPE [dbo].[ApiBatchSaveImportLogType] AS TABLE (
    [logID]           INT            NULL,
    [logType]         TINYINT        NOT NULL,
    [logName]         NVARCHAR (500) NOT NULL,
    [successfulLines] INT            NULL,
    [failedLines]     INT            NULL,
    [warningLines]    INT            NULL,
    [expectedLines]   INT            NULL,
    [processedLines]  INT            NULL,
    [createdOn]       DATETIME       NULL,
    [modifiedOn]      DATETIME       NULL);

