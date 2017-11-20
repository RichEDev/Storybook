CREATE TABLE [dbo].[currencies] (
    [currencyid]       INT      IDENTITY (1, 1) NOT NULL,
    [globalcurrencyid] INT      NULL,
    [CreatedOn]        DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [ModifiedOn]       DATETIME NULL,
    [ModifiedBy]       INT      NULL,
    [archived]         BIT      NOT NULL,
    [positiveFormat]   TINYINT  NOT NULL,
    [negativeFormat]   TINYINT  NOT NULL,
    [subAccountId]     INT      NULL
);

