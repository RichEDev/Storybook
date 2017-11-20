CREATE TABLE [dbo].[financial_status] (
    [statusid]     INT          IDENTITY (1, 1) NOT NULL,
    [description]  VARCHAR (50) NULL,
    [createdon]    DATETIME     NULL,
    [createdby]    INT          NULL,
    [modifiedon]   DATETIME     NULL,
    [modifiedby]   INT          NULL,
    [subaccountid] INT          NULL,
    [archived]     BIT          NOT NULL
);

