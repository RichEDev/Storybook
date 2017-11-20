CREATE TABLE [dbo].[importHistory] (
    [historyId]       INT      IDENTITY (1, 1)  NOT NULL,
    [importId]        INT      NOT NULL,
    [logId]           INT      NOT NULL,
    [importedDate]    DATETIME NOT NULL,
    [importStatus]    INT      NOT NULL,
    [applicationType] INT      NOT NULL,
    [dataId]          INT      NULL,
    [createdOn]       DATETIME NOT NULL,
    [modifiedOn]      DATETIME NULL
);

