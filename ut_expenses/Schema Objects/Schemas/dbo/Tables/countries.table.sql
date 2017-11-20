CREATE TABLE [dbo].[countries] (
    [countryid]       INT      IDENTITY (1, 1) NOT NULL,
    [globalcountryid] INT      NULL,
    [CreatedOn]       DATETIME NULL,
    [CreatedBy]       INT      NULL,
    [ModifiedOn]      DATETIME NULL,
    [ModifiedBy]      INT      NULL,
    [archived]        BIT      NOT NULL,
    [subAccountId]    INT      NULL
);

