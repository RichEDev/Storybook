CREATE TABLE [dbo].[corporate_cards] (
    [cardproviderid]        INT      NOT NULL,
    [claimants_settle_bill] BIT      NOT NULL,
    [CreatedOn]             DATETIME NOT NULL,
    [CreatedBy]             INT      NOT NULL,
    [ModifiedOn]            DATETIME NULL,
    [ModifiedBy]            INT      NULL,
    [allocateditem]         INT      NULL,
    [blockcash]             BIT      NOT NULL,
    [reconciled_by_admin]   BIT      NOT NULL,
    [singleclaim]           BIT      NOT NULL,
    [blockunmatched]        BIT      NOT NULL
);

