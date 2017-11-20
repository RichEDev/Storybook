CREATE TABLE [dbo].[card_providers] (
    [cardproviderid] INT           NOT NULL,
    [cardprovider]   NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [card_type]      TINYINT       NOT NULL,
    [creditcard]     BIT           NOT NULL,
    [purchasecard]   BIT           NOT NULL,
    [createdon]      DATETIME      NOT NULL,
    [createdby]      INT           NULL,
    [modifiedon]     DATETIME      NULL,
    [modifiedby]     INT           NULL
);

