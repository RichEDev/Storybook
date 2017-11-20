CREATE TABLE [dbo].[savings] (
    [savingsId]         INT          IDENTITY (1, 1) NOT NULL,
    [subAccountId]      INT          NULL,
    [contractId]        INT          NULL,
    [Reference]         VARCHAR (50) NULL,
    [savingDate]        DATETIME     NULL,
    [Amount]            FLOAT        NOT NULL,
    [Comment]           TEXT         COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [rechargeEntityId]  INT          NULL,
    [loggedByUserId]    INT          NULL,
    [loggedByTimestamp] DATETIME     NULL
);

