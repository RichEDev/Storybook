CREATE TABLE [dbo].[InterfaceLog] (
    [LogItemID]    INT            IDENTITY (1, 1) NOT NULL,
    [LogItemType]  TINYINT        NOT NULL,
    [TransferType] TINYINT        NOT NULL,
    [NHSTrustID]   INT            NOT NULL,
    [Filename]     NVARCHAR (250) NOT NULL,
    [EmailSent]    BIT            CONSTRAINT [DF_InterfaceLog_EmailSent] DEFAULT ((0)) NOT NULL,
    [LogItemBody]  NVARCHAR (MAX) NULL,
    [CreatedOn]    DATETIME       NOT NULL,
    [ModifiedOn]   DATETIME       NULL,
    CONSTRAINT [PK_InterfaceLog] PRIMARY KEY CLUSTERED ([LogItemID] ASC)
);

