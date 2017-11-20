CREATE TABLE [dbo].[corporate_cards] (
    [cardproviderid]        INT      NOT NULL,
    [claimants_settle_bill] BIT      CONSTRAINT [DF_available_corporate_cards_claimants_settle_bill] DEFAULT ((0)) NOT NULL,
    [CreatedOn]             DATETIME NOT NULL,
    [CreatedBy]             INT      NOT NULL,
    [ModifiedOn]            DATETIME NULL,
    [ModifiedBy]            INT      NULL,
    [allocateditem]         INT      NULL,
    [blockcash]             BIT      CONSTRAINT [DF_corporate_cards_blockcash] DEFAULT ((0)) NOT NULL,
    [reconciled_by_admin]   BIT      NOT NULL,
    [singleclaim]           BIT      CONSTRAINT [DF_corporate_cards_singleclaim] DEFAULT ((0)) NOT NULL,
    [blockunmatched]        BIT      CONSTRAINT [DF_corporate_cards_blockunmatched] DEFAULT ((0)) NOT NULL,
	[FileIdentifier]        NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_corporate_cards_1] PRIMARY KEY CLUSTERED ([cardproviderid] ASC)
);



