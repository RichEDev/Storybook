CREATE TABLE [dbo].[salesTax] (
    [salesTaxID]  INT          IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [locationID]  INT          NULL,
    [description] VARCHAR (50) NULL,
    [salesTax]    FLOAT        NOT NULL,
    [archived]    BIT          NOT NULL,
    [createdOn]   DATETIME     NOT NULL,
    [createdBy]   INT          NOT NULL,
    [modifiedOn]  DATETIME     NULL,
    [modifiedBy]  INT          NULL
);

