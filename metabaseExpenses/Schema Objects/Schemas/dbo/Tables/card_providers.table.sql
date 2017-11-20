CREATE TABLE [dbo].[card_providers] (
    [cardproviderid] INT           NOT NULL,
    [cardprovider]   NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [card_type]      TINYINT       NOT NULL,
    [creditcard]     BIT           CONSTRAINT [DF_card_providers_creditcard] DEFAULT ((0)) NOT NULL,
    [purchasecard]   BIT           CONSTRAINT [DF_card_providers_purchasecard] DEFAULT ((0)) NOT NULL,
    [createdon]      DATETIME      CONSTRAINT [DF_card_providers_createdon] DEFAULT (getdate()) NOT NULL,
    [createdby]      INT           NULL,
    [modifiedon]     DATETIME      NULL,
    [modifiedby]     INT           NULL,
	[AutoImport]     BIT           NULL,
    CONSTRAINT [PK_card_providers] PRIMARY KEY CLUSTERED ([cardproviderid] ASC),
    CONSTRAINT [CK_card_providers] CHECK NOT FOR REPLICATION ([card_type]=(1) OR [card_type]=(2))
);



