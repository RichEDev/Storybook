CREATE TABLE [dbo].[APILog] (
    [LogItemId]     INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]     INT             NULL,
    [NhsVpd]        NVARCHAR (3)    NULL,
    [metaBase]      NVARCHAR (20)   NULL,
    [LogItemType]   SMALLINT        NULL,
    [TransferType]  SMALLINT        NULL,
    [LogId]         INT             NULL,
    [Filename]      NVARCHAR (100)  NULL,
    [EmailSent]     BIT             NULL,
    [Message]       NVARCHAR (1000) NULL,
    [Source]        NVARCHAR (1000) NULL,
    [messageLevel]  SMALLINT        NOT NULL,
    [CreatedOn]     DATETIME        NULL,
    [LogItemreason] SMALLINT        NULL
);



GO

CREATE INDEX [IX_APILog_LogId_AccountId] ON [dbo].[APILog] (LogId, AccountId)
