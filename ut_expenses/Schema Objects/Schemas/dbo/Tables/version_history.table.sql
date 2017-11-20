CREATE TABLE [dbo].[version_history] (
    [RegistryId]   INT           NOT NULL,
    [DateStamp]    DATETIME      NULL,
    [ChangedBy]    VARCHAR (30)  NULL,
    [PreVal]       INT           NULL,
    [PostVal]      INT           NULL,
    [Comment]      VARCHAR (100) NULL,
    [Locale]       VARCHAR (60)  NULL,
    [DateObtained] DATETIME      NULL,
    [Type]         VARCHAR (30)  NULL,
    [PlusMinusQty] INT           NOT NULL,
    [Reseller]     VARCHAR (60)  NULL,
    [HistoryId]    INT           IDENTITY (1, 1) NOT NULL
);

